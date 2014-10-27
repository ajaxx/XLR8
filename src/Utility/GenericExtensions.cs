// --------------------------------------------------------------------------------
// Copyright (c) 2014, XLR8 Development
// --------------------------------------------------------------------------------
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// --------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;

namespace XLR8.Utility
{
    using Collections;

    public static class GenericExtensions
    {
        public static bool IsNullable(this Type t)
        {
            return Nullable.GetUnderlyingType(t) != null;
        }

        /// <summary>
        /// Method to check if a given class, and its superclasses and interfaces (deep), implement a given interface.
        /// </summary>
        /// <param name="clazz">to check, including all its superclasses and their interfaces and extends</param>
        /// <param name="interfaceClass">is the interface class to look for</param>
        /// <returns>
        /// true if such interface is implemented by any of the clazz or its superclasses orextends by any interface and superclasses (deep check)
        /// </returns>
        public static bool IsImplementsInterface(this Type clazz, Type interfaceClass)
        {
            if (!(interfaceClass.IsInterface))
            {
                throw new ArgumentException("Interface class passed in is not an interface");
            }

            return
                RecursiveIsImplementsInterface(clazz, interfaceClass) || 
                RecursiveSuperclassImplementsInterface(clazz, interfaceClass);
        }

        private static bool RecursiveSuperclassImplementsInterface(this Type clazz, Type interfaceClass)
        {
            var baseType = clazz.BaseType;
            if ((baseType == null) || (baseType == typeof(Object)))
            {
                return false;
            }

            return 
                RecursiveIsImplementsInterface(baseType, interfaceClass) ||
                RecursiveSuperclassImplementsInterface(baseType, interfaceClass);
        }

        private static bool RecursiveIsImplementsInterface(Type clazz, Type interfaceClass)
        {
            if (clazz == interfaceClass)
            {
                return true;
            }

            var interfaces = clazz.GetInterfaces();
            if (interfaces.Length == 0)
            {
                return false;
            }

            return interfaces
                .Select(interfaceX => RecursiveIsImplementsInterface(interfaceX, interfaceClass))
                .Any(result => result);
        }

        public static bool IsAssignableIndex(this Type t)
        {
            if (t == null)
                return false;
            if (t.IsArray)
                return false;
            if (t == typeof(XmlNode))
                return false;
            if (t == typeof(string))
                return true;
            if (IsImplementsInterface(t, typeof(System.Collections.Generic.IList<>)))
                return false;
            if (IsImplementsInterface(t, typeof(System.Collections.IList)))
                return false;
            if (t.IsArray)
                return false;

            return false;
        }

        public static Type GetIndexType(this Type t)
        {
            if (t == null)
                return null;
            if (t.IsArray)
                return t.GetElementType();
            if (t == typeof(XmlNode))
                return null;
            if (t == typeof(string))
                return typeof(char);
            if (IsImplementsInterface(t, typeof(System.Collections.Generic.IList<>)))
                return FindGenericInterface(t, typeof (System.Collections.Generic.IList<>)).GetGenericArguments()[0];
            if (IsImplementsInterface(t, typeof(System.Collections.IList)))
                return typeof(object);

            return null;
        }

        public static bool IsIndexed(this Type t)
        {
            if (t == null)
                return false;
            if (t.IsArray)
                return true;
            if (t == typeof(XmlNode))
                return false;
            if (t == typeof(string))
                return true;
            if (IsImplementsInterface(t, typeof(System.Collections.Generic.IList<>)))
                return true;
            if (IsImplementsInterface(t, typeof(System.Collections.IList)))
                return true;

            return false;
        }

        public static bool IsGenericDictionary(this Type t)
        {
            var dictType = FindGenericInterface(t, typeof(IDictionary<,>));
            return (dictType != null);
        }

        public static bool IsGenericStringDictionary(this Type t)
        {
            var dictType = FindGenericInterface(t, typeof (IDictionary<,>));
            if (dictType != null)
                return dictType.GetGenericArguments()[0] == typeof (string);
            return false;
        }

        public static bool IsGenericCollection(this Type t)
        {
            return FindGenericInterface(t, typeof (ICollection<>)) != null;
        }

        public static bool IsGenericList(this Type t)
        {
            return FindGenericInterface(t, typeof(IList<>)) != null;
        }

        public static bool IsGenericEnumerable(this Type t)
        {
            return FindGenericInterface(t, typeof(IEnumerable<>)) != null;
        }

        public static Type FindGenericList(this Type t)
        {
            return FindGenericInterface(t, typeof (IList<>));
        }

        public static Type FindGenericInterface(this Type t, Type baseInterface)
        {
            if (t.IsInterface && t.IsGenericType)
            {
                var genericType = t.GetGenericTypeDefinition();
                if (genericType == baseInterface)
                {
                    return t;
                }
            }

            foreach (var iface in t.GetInterfaces())
            {
                var subFind = FindGenericInterface(iface, baseInterface);
                if (subFind != null)
                {
                    return subFind;
                }
            }

            return null;
        }

        public static Type FindGenericDictionaryInterface(Type t)
        {
            return t.FindGenericInterface(typeof (IDictionary<,>));
        }

        public static Type FindGenericEnumerationInterface(Type t)
        {
            return t.FindGenericInterface(typeof (IEnumerable<>));
        }

        public static object FetchGenericKeyedValue<V>(this object o, string key)
        {
            var dictionary = o as IDictionary<string, V>;
            return dictionary.Get(key);
        }

        private static readonly IDictionary<Type, Func<object, string, object>> TypeFetchTable =
            new Dictionary<Type, Func<object, string, object>>();

        public static object FetchKeyedValue(object o, string key, object defaultValue)
        {
            var type = o.GetType();

            Func<object, string, object> typeFetchFunc;

            lock( TypeFetchTable ) {
                typeFetchFunc = TypeFetchTable.Get(type);
                if ( typeFetchFunc == null ) {
                    var genericDictionaryType = FindGenericDictionaryInterface(o.GetType());
                    if (genericDictionaryType == null)
                    {
                        typeFetchFunc = ((p1, p2) => defaultValue);
                    } 
                    else
                    {
                        var genericMethod = typeof(GenericExtensions).GetMethod("FetchGenericKeyedValue");
                        var specificMethod = genericMethod.MakeGenericMethod(
                            genericDictionaryType.GetGenericArguments()[1]);
                        typeFetchFunc = ((p1, p2) => specificMethod.Invoke(null, new[] {p1, p2}));
                    }

                    TypeFetchTable[type] = typeFetchFunc;
                }
            }

            return typeFetchFunc.Invoke(o, key);
        }

        private static readonly Dictionary<Type, Func<object, object, bool>> CollectionAccessorTable =
            new Dictionary<Type, Func<object, object, bool>>();

        public static Func<object, object, bool> CreateCollectionContainsAccessor(this Type t)
        {
            lock( CollectionAccessorTable ) {
                Func<object, object, bool> accessor;

                if (!CollectionAccessorTable.TryGetValue(t, out accessor))
                {
                    // Scan the object and make sure that it implements the collection interface
                    var rawInterface = FindGenericInterface(t, typeof(ICollection<>));
                    if (rawInterface == null) {
                        accessor = null;
                    } else {
                        var containMethod = rawInterface.GetMethod("Contains");
                        var exprParam1 = Expression.Parameter(t, "collection");
                        var exprParam2 = Expression.Parameter(typeof (object), "obj");
                        var exprMethod = Expression.Call(containMethod, exprParam1, exprParam2);
                        var exprLambda = Expression.Lambda<Func<object, object, bool>>(
                            exprMethod,
                            exprParam1,
                            exprParam2);
                        accessor = exprLambda.Compile();
                    }

                    CollectionAccessorTable[t] = accessor;
                }

                return accessor;
            }

        }
    }
}
