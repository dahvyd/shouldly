﻿#region License
// Copyright 2009 Buu Nguyen (http://www.buunguyen.net/blog)
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
using System.Reflection;

namespace Fasterflect.Emitter
{
    internal abstract class AttributeEmitter : BaseEmitter
    {
        protected AttributeEmitter(DelegateCache cache) : base(cache)
        {
        }

        protected MethodInfo GetPropertyGetMethod()
        {
            return GetPropertyMethod("get_", "Getter");
        }

        protected MethodInfo GetPropertySetMethod()
        {
            return GetPropertyMethod("set_", "Setter");
        }

        private MethodInfo GetPropertyMethod(string infoPrefix, string errorPrefix)
        {
            MethodInfo setMethod = callInfo.TargetType.GetMethod(infoPrefix + callInfo.Name,
                BindingFlags.Public | BindingFlags.NonPublic | ScopeFlag);
            if (setMethod == null)
                throw new MissingMemberException(errorPrefix + " method for property " + callInfo.Name + " does not exist");
            return setMethod;
        }

        protected MemberInfo GetAttribute()
        {
            if (callInfo.MemberTypes == MemberTypes.Property)
            {
                PropertyInfo member = callInfo.TargetType.GetProperty(callInfo.Name,
                    BindingFlags.NonPublic | BindingFlags.Public | ScopeFlag);
                if (member != null)
                    return member;
                throw new MissingMemberException((callInfo.IsStatic ? "Static property" : "Property") +
                    " '" + callInfo.Name + "' does not exist");
            }
            if (callInfo.MemberTypes == MemberTypes.Field)
            {
                FieldInfo field = callInfo.TargetType.GetField(callInfo.Name,
                    BindingFlags.NonPublic | BindingFlags.Public | ScopeFlag);
                if (field != null)
                    return field;
                throw new MissingFieldException((callInfo.IsStatic ? "Static field" : "Field") +
                    " '" + callInfo.Name + "' does not exist");
            }
            throw new ArgumentException(callInfo.MemberTypes + " is not supported");
        }
    }
}
