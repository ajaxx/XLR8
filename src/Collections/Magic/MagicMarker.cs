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
using System.Linq.Expressions;

using XLR8.Threading;

namespace XLR8.Collections.Magic
{
    using Utility;

    public class MagicMarker
    {
        #region Collection Factory
        private static readonly ILockable collectionFactoryTableLock = LockManager.Default.CreateLock(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IDictionary<Type, Func<object, ICollection<object>>> collectionFactoryTable =
            new Dictionary<Type, Func<object, ICollection<object>>>();

        /// <summary>
        /// Creates a factory object that produces wrappers (MagicCollection) instances for a
        /// given object.  This method is designed for those who know early on that they are
        /// going to be dealing with an opaque object that will have a true generic definition
        /// once they receive it.  However, to make life easier (relatively speaking), we would
        /// prefer to operate with a clean object rather than the type specific detail.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Func<object, ICollection<object>> GetCollectionFactory(Type t)
        {
            using(collectionFactoryTableLock.Acquire()) {
                Func<object, ICollection<object>> collectionFactory;

                if (! collectionFactoryTable.TryGetValue(t, out collectionFactory)) {
                    collectionFactory = NewCollectionFactory(t);
                    collectionFactoryTable[t] = collectionFactory;
                }

                return collectionFactory;
            }
        }

        public static ICollection<object> NewMagicCollection<V>(object o)
        {
            return o == null ? null : new MagicCollection<V>(o);
        }

        /// <summary>
        /// Constructs the factory method for the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        
        private static Func<object, ICollection<object>> NewCollectionFactory(Type t)
        {
            if (t == typeof(ICollection<object>))
                return o => (ICollection<object>)o;

            var genericType = t.FindGenericInterface(typeof (ICollection<>));
            if (genericType == null)
                return null;

            var genericArg = genericType.GetGenericArguments()[0];
            var magicActivator = typeof (MagicMarker)
                .GetMethod("NewMagicCollection", new[] {typeof (object)})
                .MakeGenericMethod(genericArg);

            // Now create a function that will create the wrapper type when
            // the generic object is presented.
            var eParam = Expression.Parameter(typeof (object), "o");
            var eBuild = Expression.Call(magicActivator, eParam);
            var eLambda = Expression.Lambda<Func<object, ICollection<object>>>(eBuild, eParam);

            // Return the compiled lambda method
            return eLambda.Compile();
        }
        #endregion

        #region List Factory
        private static readonly ILockable listFactoryTableLock = LockManager.Default.CreateLock(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IDictionary<Type, Func<object, IList<object>>> listFactoryTable =
            new Dictionary<Type, Func<object, IList<object>>>();

        public static Func<object, IList<object>> GetListFactory(Type t)
        {
            using (listFactoryTableLock.Acquire())
            {
                Func<object, IList<object>> listFactory;

                if (!listFactoryTable.TryGetValue(t, out listFactory))
                {
                    listFactory = NewListFactory(t);
                    listFactoryTable[t] = listFactory;
                }

                return listFactory;
            }
        }

        public static IList<object> NewMagicList<V>(object o)
        {
            return o == null ? null : new MagicList<V>(o);
        }

        /// <summary>
        /// Constructs the factory method for the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>

        private static Func<object, IList<object>> NewListFactory(Type t)
        {
            if (t == typeof(IList<object>))
                return o => (IList<object>)o;

            var genericType = t.FindGenericInterface(typeof(IList<>));
            if (genericType == null)
                return null;

            var genericArg = genericType.GetGenericArguments()[0];
            var magicActivator = typeof(MagicMarker)
                .GetMethod("NewMagicList", new[] { typeof(object) })
                .MakeGenericMethod(genericArg);

            // Now create a function that will create the wrapper type when
            // the generic object is presented.
            var eParam = Expression.Parameter(typeof(object), "o");
            var eBuild = Expression.Call(magicActivator, eParam);
            var eLambda = Expression.Lambda<Func<object, IList<object>>>(eBuild, eParam);

            // Return the compiled lambda method
            return eLambda.Compile();
        }
        #endregion

        #region Dictionary Factory
        private static readonly ILockable dictionaryFactoryTableLock = LockManager.Default.CreateLock(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IDictionary<Type, Func<object, IDictionary<object, object>>> dictionaryFactoryTable =
            new Dictionary<Type, Func<object, IDictionary<object, object>>>();

        /// <summary>
        /// Creates a factory object that produces wrappers (MagicStringDictionary) instances for
        /// a given object.  This method is designed for those who know early on that they are
        /// going to be dealing with an opaque object that will have a true generic definition
        /// once they receive it.  However, to make life easier (relatively speaking), we would
        /// prefer to operate with a clean object rather than the type specific detail.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Func<object, IDictionary<object, object>> GetDictionaryFactory(Type t)
        {
            using(dictionaryFactoryTableLock.Acquire())
            {
                Func<object, IDictionary<object, object>> dictionaryFactory;

                if (!dictionaryFactoryTable.TryGetValue(t, out dictionaryFactory))
                {
                    dictionaryFactory = NewDictionaryFactory(t);
                    dictionaryFactoryTable[t] = dictionaryFactory;
                }

                return dictionaryFactory;
            }
        }

        public static IDictionary<object, object> NewMagicDictionary<K, V>(object o)
        {
            return o == null ? null : new MagicDictionary<K, V>(o);
        }

        /// <summary>
        /// Constructs the factory method for the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>

        private static Func<object, IDictionary<object, object>> NewDictionaryFactory(Type t)
        {
            if (t == typeof(IDictionary<object, object>))
                return o => (IDictionary<object, object>)o;

            var genericType = t.FindGenericInterface(typeof(IDictionary<,>));
            if (genericType == null)
                return null;

            var genericKey = genericType.GetGenericArguments()[0];
            var genericArg = genericType.GetGenericArguments()[1];
            var magicActivator = typeof (MagicMarker)
                .GetMethod("NewMagicDictionary", new[] {typeof (object)})
                .MakeGenericMethod(genericKey, genericArg);

            // Now create a function that will create the wrapper type when
            // the generic object is presented.
            var eParam = Expression.Parameter(typeof(object), "o");
            var eBuild = Expression.Call(magicActivator, eParam);
            var eLambda = Expression.Lambda<Func<object, IDictionary<object, object>>>(eBuild, eParam);

            // Return the compiled lambda method
            return eLambda.Compile();
        }
        #endregion

        #region String Dictionary Factory
        private static readonly ILockable stringDictionaryFactoryTableLock = LockManager.Default.CreateLock(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly IDictionary<Type, Func<object, IDictionary<string, object>>> stringDictionaryFactoryTable =
            new Dictionary<Type, Func<object, IDictionary<string, object>>>();

        public static IDictionary<string,object> GetStringDictionary(Object o)
        {
            if ( o == null )
                return null;

            var dictionaryFactory = GetStringDictionaryFactory(o.GetType());
            if (dictionaryFactory == null)
                return null;

            return dictionaryFactory(o);
        }

        /// <summary>
        /// Creates a factory object that produces wrappers (MagicStringDictionary) instances for
        /// a given object.  This method is designed for those who know early on that they are
        /// going to be dealing with an opaque object that will have a true generic definition
        /// once they receive it.  However, to make life easier (relatively speaking), we would
        /// prefer to operate with a clean object rather than the type specific detail.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public static Func<object, IDictionary<string, object>> GetStringDictionaryFactory(Type t)
        {
            using (stringDictionaryFactoryTableLock.Acquire())
            {
                Func<object, IDictionary<string, object>> stringDictionaryFactory;

                if (!stringDictionaryFactoryTable.TryGetValue(t, out stringDictionaryFactory))
                {
                    stringDictionaryFactory = NewStringDictionaryFactory(t);
                    stringDictionaryFactoryTable[t] = stringDictionaryFactory;
                }

                return stringDictionaryFactory;
            }
        }

        public static IDictionary<string, object> NewMagicStringDictionary<V>(object o)
        {
            return o == null ? null : new MagicStringDictionary<V>(o);
        }

        /// <summary>
        /// Constructs the factory method for the given type.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>

        private static Func<object, IDictionary<string, object>> NewStringDictionaryFactory(Type t)
        {
            if (t == typeof(IDictionary<string, object>))
                return o => (IDictionary<string, object>)o;

            var genericType = t.FindGenericInterface(typeof(IDictionary<,>));
            if (genericType == null)
                return null;

            var genericKey = genericType.GetGenericArguments()[0];
            if (genericKey != typeof(string))
                return null;

            var genericArg = genericType.GetGenericArguments()[1];
            var magicActivator = typeof(MagicMarker)
                .GetMethod("NewMagicStringDictionary", new[] { typeof(object) })
                .MakeGenericMethod(genericArg);

            // Now create a function that will create the wrapper type when
            // the generic object is presented.
            var eParam = Expression.Parameter(typeof(object), "o");
            var eBuild = Expression.Call(magicActivator, eParam);
            var eLambda = Expression.Lambda<Func<object, IDictionary<string, object>>>(eBuild, eParam);

            // Return the compiled lambda method
            return eLambda.Compile();
        }

        #endregion
    }
}
