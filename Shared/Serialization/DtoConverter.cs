using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web.Script.Serialization;

namespace SoftwareMind.Shared.Serialization
{
    public class DtoConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, System.Web.Script.Serialization.JavaScriptSerializer serializer)
        {
            if (dictionary == null)
            {
                return null;
            }

            List<String> memberNames = new List<String>(dictionary.Keys);
            object o = Activator.CreateInstance(type);

            if (type != null && !type.IsAssignableFrom(o.GetType()))
            {
                ConstructorInfo constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
                if (constructorInfo == null)
                {
                    throw new MissingMethodException(String.Format(CultureInfo.InvariantCulture, "JSON_NoConstructor {0}", type.FullName));
                }

                throw new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "JSON_DeserializerTypeMismatch {0}", type.FullName));
            }

            foreach (string memberName in memberNames)
            {
                object propertyValue = dictionary[memberName];
                // Assign the value into a property or field of the object
                if (!AssignToPropertyOrField(propertyValue, o, memberName, serializer))
                {
                    return null;
                }
            }

            return o;
        }

        public override IDictionary<string, object> Serialize(object obj, System.Web.Script.Serialization.JavaScriptSerializer _serializer)
        {
            var serializer = (JavaScriptSerializer)_serializer;
            if (obj == null)
            {
                return null;
            }

            IDictionary<string, object> result = new Dictionary<string, object>();
            Type type = obj.GetType();

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fieldInfo in fields)
            {
                // Ignore all fields marked as [ScriptIgnore]
                if (fieldInfo.IsDefined(typeof(ScriptIgnoreAttribute), true /*inherits*/))
                {
                    continue;
                }

                object value = FieldInfoGetValue(fieldInfo, obj);
                if (serializer.SkipNulls && this.CanSkip(value, fieldInfo.FieldType))
                {
                    continue;
                }

                result[fieldInfo.Name] = value;
            }

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
            foreach (PropertyInfo propInfo in props)
            {
                // Ignore all properties marked as [ScriptIgnore]
                if (propInfo.IsDefined(typeof(ScriptIgnoreAttribute), true /*inherits*/))
                {
                    continue;
                }

                MethodInfo getMethodInfo = propInfo.GetGetMethod();

                // Skip property if it has no get
                if (getMethodInfo == null)
                {
                    continue;
                }

                // Ignore indexed properties
                if (getMethodInfo.GetParameters().Length > 0)
                {
                    continue;
                }

                object value = MethodInfoInvoke(getMethodInfo, obj, null);
                if (serializer.SkipNulls && this.CanSkip(value, propInfo.PropertyType))
                {
                    continue;
                }

                result[propInfo.Name] = value;
            }

            return result;
        }

        private bool CanSkip(object value, Type type)
        {
            if (value == null)
            {
                return true;
            }

            if (type.IsValueType)
            {
                return value.Equals(Activator.CreateInstance(type));
            }

            return false;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                yield return typeof(OzirDTO);
            }
        }

        #region Helpers

        private static ReflectionPermission memberAccessPermission = null;
        private static ReflectionPermission restrictedMemberAccessPermission = null;

        private static ReflectionPermission MemberAccessPermission
        {
            get
            {
                if (memberAccessPermission == null)
                {
                    memberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.MemberAccess);
                }
                return memberAccessPermission;
            }
        }

        private static ReflectionPermission RestrictedMemberAccessPermission
        {
            get
            {
                if (restrictedMemberAccessPermission == null)
                {
                    restrictedMemberAccessPermission = new ReflectionPermission(ReflectionPermissionFlag.RestrictedMemberAccess);
                }
                return restrictedMemberAccessPermission;
            }
        }

        private static void DemandReflectionAccess(Type type)
        {
            try
            {
                MemberAccessPermission.Demand();
            }
            catch (SecurityException)
            {
                DemandGrantSet(type.Assembly);
            }
        }

        private static bool GenericArgumentsAreVisible(MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                Type[] parameterTypes = method.GetGenericArguments();
                foreach (Type type in parameterTypes)
                {
                    if (!type.IsVisible)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <devdoc>
        ///     This helper method provides safe access to FieldInfo's GetValue method.
        /// </devdoc>
        internal static object FieldInfoGetValue(FieldInfo field, object target)
        {
            Type type = field.DeclaringType;
            if (type == null)
            {
                // Type is null for Global fields.
                if (!field.IsPublic)
                {
                    DemandGrantSet(field.Module.Assembly);
                }
            }
            else if (!(type != null && type.IsVisible && field.IsPublic))
            {
                DemandReflectionAccess(type);
            }
            return field.GetValue(target);
        }

        /// <devdoc>
        ///     This helper method provides safe access to MethodInfo's Invoke method.
        /// </devdoc>
        internal static object MethodInfoInvoke(MethodInfo method, object target, object[] args)
        {
            Type type = method.DeclaringType;
            if (type == null)
            {
                // Type is null for Global methods. In this case we would need to demand grant set on
                // the containing assembly for internal methods.
                if (!(method.IsPublic && GenericArgumentsAreVisible(method)))
                {
                    DemandGrantSet(method.Module.Assembly);
                }
            }
            else if (!(type.IsVisible && method.IsPublic && GenericArgumentsAreVisible(method)))
            {
                // this demand is required for internal types in system.dll and its friend assemblies.
                DemandReflectionAccess(type);
            }
            return method.Invoke(target, args);
        }

        [SecuritySafeCritical]
        private static void DemandGrantSet(Assembly assembly)
        {
            PermissionSet targetGrantSet = assembly.PermissionSet;
            targetGrantSet.AddPermission(RestrictedMemberAccessPermission);
            targetGrantSet.Demand();
        }

        private static bool AssignToPropertyOrField(object propertyValue, object o, string memberName, System.Web.Script.Serialization.JavaScriptSerializer serializer)
        {
            Type serverType = o.GetType();
            // First, look for a property
            PropertyInfo propInfo = serverType.GetProperty(memberName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);

            if (propInfo != null)
            {
                // Ignore it if the property has no setter
                MethodInfo setter = propInfo.GetSetMethod();
                if (setter != null)
                {
                    propertyValue = serializer.ConvertToType(propertyValue, propInfo.PropertyType);

                    // Set the property in the object
                    setter.Invoke(o, new Object[] { propertyValue });
                    return true;
                }
            }

            // We couldn't find a property, so try a field
            FieldInfo fieldInfo = serverType.GetField(memberName, BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public);

            if (fieldInfo != null)
            {
                propertyValue = serializer.ConvertToType(propertyValue, fieldInfo.FieldType);

                // Set the field in the object
                fieldInfo.SetValue(o, propertyValue);
                return true;
            }

            // not a property or field, so it is ignored
            return true;
        }

        #endregion
    }
}
