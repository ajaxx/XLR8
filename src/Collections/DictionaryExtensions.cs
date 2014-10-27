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
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace XLR8.Collections
{
    public static class DictionaryExtensions
    {
        public static IDictionary<TK, TV> AsSyncDictionary<TK, TV>(this IDictionary<TK, TV> dictionary)
            where TK : class
        {
            if (dictionary is ConcurrentDictionary<TK, TV>)
            {
                return dictionary;
            }

            return new ConcurrentDictionary<TK, TV>(dictionary);
        }

        /// <summary>
        /// Removes the item from the dictionary that is associated with
        /// the specified key.  Returns the value that was found at that
        /// location and removed or the defaultValue.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">Search key into the dictionary</param>
        /// <param name="value">The value removed from the dictionary (if found).</param>
        /// <returns></returns>

        public static bool Remove<K, V>(this IDictionary<K, V> dictionary, K key, out V value)
        {
            dictionary.TryGetValue(key, out value);
            return dictionary.Remove(key);
        }

        /// <summary>
        /// Removes the item from the dictionary that is associated with
        /// the specified key.  The item if found is returned; if not,
        /// default(V) is returned.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>

        public static V Pluck<K, V>(this IDictionary<K, V> dictionary, K key)
        {
            V tempItem;

            return dictionary.Remove(key, out tempItem)
                    ? tempItem
                    : default(V);
        }

        /// <summary>
        /// Fetches the value associated with the specified key.
        /// If no value can be found, then the defaultValue is
        /// returned.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>

        public static V Get<K, V>(this IDictionary<K, V> dictionary, K key, V defaultValue)
        {
            V returnValue;
            if (!dictionary.TryGetValue(key, out returnValue))
            {
                returnValue = defaultValue;
            }

            return returnValue;
        }

        /// <summary>
        /// Fetches the value associated with the specified key.
        /// If no value can be found, then default(V) is returned.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>

        public static V Get<K, V>(this IDictionary<K, V> dictionary, K key)
        {
            return Get(dictionary, key, default(V));
        }

        /// <summary>
        /// Sets the given key in the dictionary.  If the key
        /// already exists, then it is remapped to thenew value.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>

        public static void Put<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            dictionary[key] = value;
        }

        /// <summary>
        /// Sets the given key in the dictionary.  If the key
        /// already exists, then it is remapped to the new value.
        /// If a value was previously mapped it is returned.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>

        public static V Push<K, V>(this IDictionary<K, V> dictionary, K key, V value)
        {
            V temp;
            dictionary.TryGetValue(key, out temp);
            dictionary[key] = value;
            return temp;
        }

        /// <summary>
        /// Puts all values from the source dictionary into
        /// this dictionary.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="source">The source.</param>

        public static void PutAll<K, V>(this IDictionary<K, V> dictionary, IEnumerable<KeyValuePair<K, V>> source)
        {
            foreach (KeyValuePair<K, V> kvPair in source)
            {
                dictionary[kvPair.Key] = kvPair.Value;
            }
        }

        /// <summary>
        /// Puts all values from the source dictionary into this dictionary.  This variation
        /// of the method allows the values to be transformed from one type to another.
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="source">The source.</param>
        /// <param name="transformer">The transformer.</param>

        public static void PutAll<K, V, T>(this IDictionary<K, V> dictionary, IEnumerable<KeyValuePair<K, T>> source, Func<T, V> transformer)
        {
            foreach (var kvPair in source)
            {
                dictionary[kvPair.Key] = transformer(kvPair.Value);
            }
        }

        /// <summary>
        /// Returns the first value in the enumeration of values
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>

        public static V FirstValue<K, V>(this IDictionary<K, V> dictionary)
        {
            IEnumerator<KeyValuePair<K, V>> kvPairEnum = dictionary.GetEnumerator();
            kvPairEnum.MoveNext();
            return kvPairEnum.Current.Value;
        }
    }
}
