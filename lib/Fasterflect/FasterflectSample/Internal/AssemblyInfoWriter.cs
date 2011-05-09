﻿#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://fasterflect.codeplex.com/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Fasterflect;

namespace FasterflectSample.Internal
{
	/// <summary>
	/// Internally use by Fasterflect team
	/// </summary>
	internal class AssemblyInfoWriter
	{
		//static void Main()
		//{
		//    Console.WriteLine( AssemblyInfoWriter.ListExtensions( Assembly.GetAssembly( typeof(Flags) ) ) );
		//}

		public static string ListExtensions( Assembly assembly )
		{
			var sb = new StringBuilder();
			sb.AppendFormat( "=== Extension methods (all):{0}", Environment.NewLine );
			IList<Type> types = assembly.Types( Flags.PartialNameMatch, "Extensions" );
			types.ForEach( t => Write( sb, t, t.Methods( Flags.Static | Flags.Public ).OrderBy( m => m.Name ).ToList() ) );
			return sb.ToString();
		}

		public static string ListExtensionMethodsWithSuperfluousOverloads( Assembly assembly )
		{
			var sb = new StringBuilder();
			sb.AppendFormat( "=== Extension methods that are superfluous:{0}", Environment.NewLine );
			IList<Type> types = assembly.Types( Flags.PartialNameMatch, "Extensions" );
			types.ForEach( t => Write( sb, t, t.Methods( Flags.Static | Flags.Public )
			                                  	.OrderBy( m => m.Name )
			                                  	.Where( m => IsSuperfluous( m, t.Methods( Flags.Static | Flags.Public, m.Name ) ) )
			                                  	.ToList() ) );
			return sb.ToString();
		}

		private static bool IsSuperfluous( MethodInfo method, IEnumerable<MethodInfo> methods )
		{
			IList<ParameterInfo> parameters = method.Parameters();
			bool lastIsParams = HasParamsParameter( parameters );
			if( lastIsParams )
			{
				return false;
			}
			return methods.Where( m => method.IsGenericMethod
			                           	? m.IsGenericMethod &&
			                           	  method.GetGenericArguments().SequenceEqual( m.GetGenericArguments() )
			                           	: !m.IsGenericMethod )
				.Select( m => m.Parameters() )
				.Where( ps => parameters.Count == ps.Count - 1 && HasParamsParameter( ps ) )
				.Any(
					ps =>
					parameters.Select( p => p.ParameterType ).SequenceEqual( ps.Take( parameters.Count ).Select( p => p.ParameterType ) ) );
		}

		public static string ListExtensionMethodsWhereParametersViolateConventions( Assembly assembly )
		{
			var sb = new StringBuilder();
			sb.AppendFormat( "=== Extension methods with naming violations:{0}", Environment.NewLine );
			IList<Type> types = assembly.Types( Flags.PartialNameMatch, "Extensions" );
			types.ForEach( t => Write( sb, t, t.Methods( Flags.Static | Flags.Public )
			                                  	.OrderBy( m => m.Name )
			                                  	.Where(
			                                  		m => IsParameterViolation( typeof(Flags), "bindingFlags", true, m.Parameters() ) )
			                                  	.ToList() ) );
			return sb.ToString();
		}

		private static bool IsParameterViolation( Type parameterType, string expectedName, bool shouldBeLast,
		                                          IList<ParameterInfo> parameters )
		{
			int lastIndex = parameters.Count - 1;
			bool lastIsParams = HasParamsParameter( parameters );
			for( int i = 0; i < parameters.Count; i++ )
			{
				ParameterInfo parameter = parameters[ i ];
				if( parameter.ParameterType == parameterType )
				{
					bool valid = parameter.Name == expectedName;
					valid &= !shouldBeLast || (i == lastIndex || (lastIsParams && i == lastIndex - 1));
					if( !valid )
					{
						return true;
					}
				}
			}
			return false;
		}

		private static bool HasParamsParameter( IList<ParameterInfo> parameters )
		{
			int lastIndex = parameters.Count - 1;
			bool lastIsParams = lastIndex >= 0 &&
			                    parameters[ lastIndex ].ParameterType.IsArray &&
			                    parameters[ lastIndex ].HasAttribute<ParamArrayAttribute>();
			return lastIsParams;
		}

		private static void Write( StringBuilder sb, Type type, IList<MethodInfo> methods )
		{
			if( methods.Count > 0 )
			{
				sb.AppendFormat( "{1}--- {0}{1}", type.Name, Environment.NewLine );
				methods.ForEach( m => Write( sb, m, m.Parameters() ) );
			}
		}

		private static void Write( StringBuilder sb, MethodInfo method, IList<ParameterInfo> parameters )
		{
			sb.AppendFormat( "{0} ", GetTypeName( method.ReturnType ) ); // GetTypeName(method.ReturnType));
			sb.AppendFormat( "{0}{1}( ", method.Name, GetGenericParameterText( method ) );
			ParameterInfo first = parameters.FirstOrDefault();
			ParameterInfo last = parameters.LastOrDefault();
			bool addPrefix = method.IsStatic && method.DeclaringType.Name.EndsWith( "Extensions" );
			parameters.ForEach( p => sb.AppendFormat( "{0}{1} {2}{3}",
			                                          p == first && addPrefix ? "this " : "",
			                                          GetType( p ), p.Name, p == last ? "" : ", " ) );
			sb.AppendFormat( " );{0}", Environment.NewLine );
		}

		private static string GetGenericParameterText( MethodInfo method )
		{
			if( ! method.ContainsGenericParameters )
			{
				return string.Empty;
			}
			Type[] genericParameters = method.GetGenericArguments();
			string args = string.Join( ",", genericParameters.Select( GetTypeName ) );
			return "<" + args + ">";
		}

		private static string GetType( ParameterInfo parameter )
		{
			bool isParams = parameter.ParameterType.IsArray && parameter.HasAttribute<ParamArrayAttribute>();
			string prefix = isParams ? "params " : string.Empty;
			return prefix + GetTypeName( parameter.ParameterType );
		}

		private static string GetTypeName( Type type )
		{
			if( type.IsArray )
			{
				return string.Format( "{0}[]", GetTypeName( type.GetElementType() ) );
			}
			if( type.ContainsGenericParameters || type.IsGenericType )
			{
				if( type.BaseType == typeof(Nullable<>) )
				{
					return GetCSharpTypeName( type.GetGenericArguments().Single().Name ) + "?";
				}
				int index = type.Name.IndexOf( "`" );
				string genericTypeName = index > 0 ? type.Name.Substring( 0, index ) : type.Name;
				string genericArgs = string.Join( ",", type.GetGenericArguments().Select( GetTypeName ) );
				return genericArgs.Length == 0 ? genericTypeName : genericTypeName + "<" + genericArgs + ">";
			}
			return GetCSharpTypeName( type.Name );
		}

		private static string GetCSharpTypeName( string typeName )
		{
			switch( typeName )
			{
				case "String":
				case "Object":
				case "Void":
				case "Byte":
				case "Double":
				case "Decimal":
					return typeName.ToLower();
				case "Int16":
					return "short";
				case "Int32":
					return "int";
				case "Int64":
					return "long";
				case "Single":
					return "float";
				case "Boolean":
					return "bool";
				default:
					return typeName;
			}
		}
	}
}
