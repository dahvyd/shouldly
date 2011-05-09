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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Fasterflect;
using FasterflectTest.SampleModel.Animals;
using FasterflectTest.SampleModel.Animals.Attributes;
using FasterflectTest.SampleModel.Animals.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FasterflectTest.Lookup
{
    [TestClass]
    public class AttributeTest
    {
        #region Enumerations
        #region Attribute<T>()
        [TestMethod]
        public void TestFindSingleAttributeOnEnum_Generic()
        {
            CodeAttribute attr = typeof(Climate).Attribute<CodeAttribute>();
            Assert.IsNotNull( attr );
            Assert.AreEqual( "Temperature", attr.Code );
        }

        [TestMethod]
        public void TestFindSingleAttributeOnEnumField_Generic()
        {
            CodeAttribute attr = Climate.Hot.Attribute<CodeAttribute>();
            Assert.IsNotNull( attr );
            Assert.AreEqual( "Hot", attr.Code );
            attr = Climate.Any.Attribute<CodeAttribute>();
            Assert.IsNotNull( attr );
            Assert.AreEqual( "Any", attr.Code );
        }
        #endregion

        #region Attributes()
        [TestMethod]
        public void TestFindAllAttributesOnEnum()
        {
            IList<Attribute> attrs = typeof(Climate).Attributes();
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 2, attrs.Count );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnEnum()
        {
            IList<Attribute> attrs = typeof(Climate).Attributes( typeof(CodeAttribute) );
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 1, attrs.Count );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnEnum_Generic()
        {
            IList<CodeAttribute> attrs = typeof(Climate).Attributes<CodeAttribute>();
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 1, attrs.Count );
        }

        [TestMethod]
        public void TestFindAllAttributesOnEnumField()
        {
            IList<Attribute> attrs = MovementCapabilities.Land.Attributes();
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 0, attrs.Count );

            attrs = Climate.Cold.Attributes();
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 1, attrs.Count );
            var codes = attrs.Cast<CodeAttribute>();
            Assert.IsNotNull( codes.FirstOrDefault( a => a.Code == "Cold" ) );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnEnumField()
        {
            IList<Attribute> attrs = Climate.Hot.Attributes( typeof(ConditionalAttribute) );
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 0, attrs.Count );

            attrs = Climate.Hot.Attributes( typeof(CodeAttribute) );
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 1, attrs.Count );
            var codes = attrs.Cast<CodeAttribute>();
            Assert.IsNotNull( codes.FirstOrDefault( a => a.Code == "Hot" ) );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnEnumField_Generic()
        {
            IList<ConditionalAttribute> empty = Climate.Hot.Attributes<ConditionalAttribute>();
            Assert.IsNotNull( empty );
            Assert.AreEqual( 0, empty.Count );

            IList<CodeAttribute> attrs = Climate.Hot.Attributes<CodeAttribute>();
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 1, attrs.Count );
            Assert.IsNotNull( attrs.FirstOrDefault( a => a.Code == "Hot" ) );
        }
        #endregion
        #endregion

        #region Find attributes on types
        #region Attribute() and Attribute<T>()
        [TestMethod]
        public void TestFindFirstAttributeOnType()
        {
            Attribute attr = typeof(Giraffe).Attribute();
            Assert.IsNotNull( attr );
			Assert.IsInstanceOfType( attr, typeof(ZoneAttribute) );
        }

		[TestMethod]
        public void TestFindSpecificAttributeOnType()
        {
            CodeAttribute code = typeof(Lion).Attribute<CodeAttribute>();
            Assert.IsNull( code );

            ZoneAttribute zone = typeof(Lion).Attribute<ZoneAttribute>();
            Assert.IsNotNull( zone );
            Assert.AreEqual( Zone.Savannah, zone.Zone );
        }
        #endregion

        #region Attributes()
        [TestMethod]
        public void TestFindAllAttributesOnType()
        {
            IList<Attribute> attrs = typeof(Lion).Attributes();
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 3, attrs.Count );
            Assert.IsTrue( attrs[ 0 ] is ZoneAttribute || attrs[ 0 ] is SerializableAttribute || attrs[ 0 ] is DebuggerDisplayAttribute );
            Assert.IsTrue( attrs[ 1 ] is ZoneAttribute || attrs[ 1 ] is SerializableAttribute || attrs[ 1 ] is DebuggerDisplayAttribute );
            Assert.IsTrue( attrs[ 2 ] is ZoneAttribute || attrs[ 2 ] is SerializableAttribute || attrs[ 2 ] is DebuggerDisplayAttribute );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnType()
        {
            IList<Attribute> attrs = typeof(Lion).Attributes( typeof(CodeAttribute) );
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 0, attrs.Count );

            attrs = typeof(Lion).Attributes( typeof(ZoneAttribute) );
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 1, attrs.Count );
            Assert.IsNotNull( attrs.Cast<ZoneAttribute>().FirstOrDefault( a => a.Zone == Zone.Savannah ) );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnType_Generic()
        {
            IList<CodeAttribute> empty = typeof(Lion).Attributes<CodeAttribute>();
            Assert.IsNotNull( empty );
            Assert.AreEqual( 0, empty.Count );

            IList<ZoneAttribute> attrs = typeof(Lion).Attributes<ZoneAttribute>();
            Assert.IsNotNull( attrs );
            Assert.AreEqual( 1, attrs.Count );
            Assert.IsNotNull( attrs.FirstOrDefault( a => a.Zone == Zone.Savannah ) );
        }
        #endregion

        #region TypesWith()
        [TestMethod]
        public void TestFindTypesWith()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            IList<Type> types = assembly.TypesWith( typeof(CodeAttribute) );
            Assert.IsNotNull( types );
            Assert.AreEqual( 1, types.Count );
            Assert.AreEqual( typeof(Climate), types[ 0 ] );
        }

        [TestMethod]
        public void TestFindTypesWithGeneric()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            IList<Type> types = assembly.TypesWith<CodeAttribute>();
            Assert.IsNotNull( types );
            Assert.AreEqual( 1, types.Count );
            Assert.AreEqual( typeof(Climate), types[ 0 ] );
        }
        #endregion

        #region FieldsAndPropertiesWith()
        [TestMethod]
        public void TestFindFieldsAndPropertiesWith_NoMatchShouldReturnEmptyList()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.FieldsAndPropertiesWith( Flags.InstanceAnyVisibility, typeof(ZoneAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 0, members.Count );
        }

        [TestMethod]
        public void TestFindFieldsAndPropertiesWith_InstanceFieldShouldIncludeInheritedPrivateField()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.FieldsAndPropertiesWith( Flags.InstanceAnyVisibility, typeof(DefaultValueAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 2, members.Count );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "Name" ) );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "birthDay" ) );
			
            members = type.FieldsAndPropertiesWith( Flags.Instance | Flags.NonPublic, typeof(CodeAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 2, members.Count );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "id" ) );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "lastMealTime" ) );
        }

        [TestMethod]
        public void TestFinFieldsAndPropertiesWithWith_DefaultFlagsShouldBeInstanceAnyVisibility()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.FieldsAndPropertiesWith( typeof(DefaultValueAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 2, members.Count );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "Name" ) );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "birthDay" ) );
       }
        #endregion

        #region MembersWith()
        [TestMethod]
        public void TestFindMembersWith_EmptyOrNullAttributeTypeListShouldReturnAll()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith( MemberTypes.Field, Flags.InstanceAnyVisibility );
            Assert.IsNotNull( members );
            Assert.AreEqual( 7, members.Count );
			
            members = type.MembersWith( MemberTypes.Field, Flags.InstanceAnyVisibility, null );
            Assert.IsNotNull( members );
            Assert.AreEqual( 7, members.Count );
        }

        [TestMethod]
        public void TestFindMembersWith_InstanceFieldShouldIncludeInheritedPrivateField()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith( MemberTypes.Field, Flags.InstanceAnyVisibility, typeof(CodeAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 2, members.Count );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "id" ) );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "lastMealTime" ) );
			
            members = type.MembersWith( MemberTypes.Field, Flags.NonPublic | Flags.Instance, typeof(DefaultValueAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 1, members.Count );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "birthDay" ) );
        }

        [TestMethod]
        public void TestFindMembersWith_InstanceFieldShouldIncludeInheritedPrivateField_Generic()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith<CodeAttribute>( MemberTypes.Field, Flags.InstanceAnyVisibility );
            Assert.IsNotNull( members );
            Assert.AreEqual( 2, members.Count );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "id" ) );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "lastMealTime" ) );
        }

        [TestMethod]
        public void TestFindMembersWith_DefaultFlagsShouldBeInstanceAnyVisibility()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith( MemberTypes.Field, typeof(CodeAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 2, members.Count );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "id" ) );
            Assert.IsNotNull( members.FirstOrDefault( m => m.Name == "lastMealTime" ) );
        }
        #endregion

        #region MembersAndAttributes()
        [TestMethod]
        public void TestFindMembersAndAttributes()
        {
            var members = typeof(Lion).MembersAndAttributes( MemberTypes.Field, Flags.InstanceAnyVisibility, typeof(CodeAttribute) );
            Assert.IsNotNull( members );
            Assert.AreEqual( 2, members.Count );
            foreach( var item in members )
            {
                Assert.IsTrue( item.Key.Name == "id" || item.Key.Name == "lastMealTime" );
                Assert.AreEqual( 1, item.Value.Count );
                Assert.IsTrue( item.Value[ 0 ] is CodeAttribute );
            }
        }

        [TestMethod]
        public void TestFindMembersAndAttributes_DefaultFlagsShouldBeInstanceAnyVisibility()
        {
            var expectedMembers = typeof(Lion).MembersAndAttributes( MemberTypes.Field, Flags.InstanceAnyVisibility, typeof(CodeAttribute) );
            Assert.IsNotNull( expectedMembers );
            Assert.AreEqual( 2, expectedMembers.Count );
            var members = typeof(Lion).MembersAndAttributes( MemberTypes.Field, typeof(CodeAttribute) );
            Assert.IsNotNull( expectedMembers );
			foreach( var item in expectedMembers )
            {
                Assert.IsTrue( members.ContainsKey( item.Key ) );
            	CollectionAssert.AreEquivalent( item.Value, members[ item.Key ] );
            }
        }
        #endregion
        #endregion

        #region Find attributes on members
        #region Attribute<T>()
        [TestMethod]
        public void TestFindSpecificAttributeOnField()
        {
            // declared field
            FieldInfo field = typeof(Lion).Field( "lastMealTime", Flags.InstanceAnyVisibility | Flags.DeclaredOnly );
            Assert.IsNotNull( field );
            Assert.IsNotNull( field.Attribute<CodeAttribute>() );
            // inherited field
            field = typeof(Lion).Field( "birthDay", Flags.InstanceAnyVisibility );
            Assert.IsNotNull( field );
            field = typeof(Lion).Field( "birthDay", Flags.InstanceAnyVisibility );
            Assert.IsNotNull( field );
            Assert.IsNull( field.Attribute<CodeAttribute>() );
            Assert.IsNotNull( field.Attribute<DefaultValueAttribute>() );
        }

        [TestMethod]
        public void TestFindSpecificAttributeOnProperty()
        {
            // inherited property (without attributes)
            PropertyInfo info = typeof(Lion).Property( "ID" );
            Assert.IsNotNull( info );
            Assert.IsNull( info.Attribute<CodeAttribute>() );
            // declared property
            info = typeof(Lion).Property( "Name" );
            Assert.IsNotNull( info );
            Assert.IsNotNull( info.Attribute<CodeAttribute>() );
            Assert.IsNotNull( info.Attribute<DefaultValueAttribute>() );
            // inherited property
            info = typeof(Lion).Property( "MovementCapabilities" );
            Assert.IsNotNull( info );
            Assert.IsNotNull( info.Attribute<CodeAttribute>() );
        }
        #endregion

        #region Attributes()
        [TestMethod]
        public void TestFindAllAttributesOnField()
        {
            // declared field
            FieldInfo info = typeof(Lion).Field( "lastMealTime" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 1, info.Attributes().Count );
            // inherited field
            info = typeof(Lion).Field( "id" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 1, info.Attributes().Count );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnField()
        {
            // declared field
            FieldInfo info = typeof(Lion).Field( "lastMealTime" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 1, info.Attributes<CodeAttribute>().Count );
            // inherited field
            info = typeof(Lion).Field( "id" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 1, info.Attributes<CodeAttribute>().Count );
        }

        [TestMethod]
        public void TestFindAllAttributesOnProperty()
        {
            // inherited property (without attributes)
            PropertyInfo info = typeof(Lion).Property( "ID" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 0, info.Attributes().Count );
            // declared property
            info = typeof(Lion).Property( "Name" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 2, info.Attributes().Count );
            // inherited property
            info = typeof(Lion).Property( "MovementCapabilities" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 1, info.Attributes().Count );
        }

        [TestMethod]
        public void TestFindSpecificAttributesOnProperty()
        {
            // inherited property (without attributes)
            PropertyInfo info = typeof(Lion).Property( "ID" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 0, info.Attributes<CodeAttribute>().Count );
            // declared property
            info = typeof(Lion).Property( "Name" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 1, info.Attributes<CodeAttribute>().Count );
            Assert.AreEqual( 1, info.Attributes<DefaultValueAttribute>().Count );
            // inherited property
            info = typeof(Lion).Property( "MovementCapabilities" );
            Assert.IsNotNull( info );
            Assert.AreEqual( 1, info.Attributes<CodeAttribute>().Count );
        }
        #endregion

		#region HasAttribute and relatives
        [TestMethod]
        public void TestHasAttributeOnField()
        {
            FieldInfo info = typeof(Lion).Field( "lastMealTime" );
            Assert.IsNotNull( info );
            Assert.IsFalse( info.HasAttribute( typeof(DefaultValueAttribute) ) );
            Assert.IsTrue( info.HasAttribute( typeof(CodeAttribute) ) );
        }

		[TestMethod]
        public void TestHasAttributeOnField_Generic()
        {
            FieldInfo info = typeof(Lion).Field( "lastMealTime" );
            Assert.IsNotNull( info );
            Assert.IsFalse( info.HasAttribute<DefaultValueAttribute>() );
            Assert.IsTrue( info.HasAttribute<CodeAttribute>() );
        }

		[TestMethod]
        public void TestHasAnyAttribute()
        {
            FieldInfo field = typeof(Lion).Field( "lastMealTime" );
            Assert.IsNotNull( field );
            Assert.IsTrue( field.HasAnyAttribute( null ) );
            Assert.IsTrue( field.HasAnyAttribute( typeof(CodeAttribute) ) );
            Assert.IsFalse( field.HasAnyAttribute( typeof(DefaultValueAttribute) ) );
            Assert.IsTrue( field.HasAnyAttribute( typeof(CodeAttribute), typeof(DefaultValueAttribute) ) );
            
			PropertyInfo property = typeof(Lion).Property( "Name" );
            Assert.IsNotNull( property );
            Assert.IsTrue( property.HasAnyAttribute( typeof(CodeAttribute), typeof(DefaultValueAttribute) ) );
        }

		[TestMethod]
        public void TestHasAllAttributes()
        {
            FieldInfo field = typeof(Lion).Field( "lastMealTime" );
            Assert.IsNotNull( field );
            Assert.IsTrue( field.HasAllAttributes() );
            Assert.IsTrue( field.HasAllAttributes( null ) );
            Assert.IsFalse( field.HasAllAttributes( typeof(CodeAttribute), typeof(DefaultValueAttribute) ) );
            
			PropertyInfo property = typeof(Lion).Property( "Name" );
            Assert.IsNotNull( property );
            Assert.IsTrue( property.HasAllAttributes( typeof(CodeAttribute), typeof(DefaultValueAttribute) ) );
        }
		#endregion
        #endregion
	}
}


