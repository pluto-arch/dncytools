using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnetydd.Tools.Core.Extension
{
    public static class ObjectTypeExtensions
    {
        public static bool IsOneOf(this Type type, params Type[] possibleTypes)
        {
            return possibleTypes.Any(possibleType => possibleType == type);
        }

        public static bool IsAssignableTo(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Is Constructed From <see cref="possibleBaseTypes"/>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="possibleBaseTypes"></param>
        /// <returns></returns>
        public static bool IsAssignableToOneOf(this Type type, params Type[] possibleBaseTypes)
        {
            return possibleBaseTypes.Any(possibleBaseType => possibleBaseType.IsAssignableFrom(type));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <param name="constructedType"></param>
        /// <returns></returns>
        public static bool IsConstructedFrom(this Type type, Type genericType, out Type? constructedType)
        {
            constructedType = new[] { type }
                .Union(type.GetInheritanceChain())
                .Union(type.GetInterfaces())
#if NET40
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);
#else
                .FirstOrDefault(i => i.IsConstructedGenericType && i.GetGenericTypeDefinition() == genericType);
#endif

            return constructedType != null;
        }

        /// <summary>
        /// Is Reference Or Nullable Type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsReferenceOrNullableType(this Type type)
        {
            return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
        }


        /// <summary>
        /// 获取默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? GetDefaultValue(this Type type)
        {
            return type.IsValueType
                ? Activator.CreateInstance(type)
                : null;
        }

        /// <summary>
        /// 获取继承链
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type[] GetInheritanceChain(this Type type)
        {
            var inheritanceChain = new List<Type>();

            var current = type;
            while (current.BaseType != null)
            {
                inheritanceChain.Add(current.BaseType);
                current = current.BaseType;
            }

            return inheritanceChain.ToArray();
        }
    }
}

