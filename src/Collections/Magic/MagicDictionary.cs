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
using System.Collections;
using System.Collections.Generic;

namespace XLR8.Collections.Magic
{
    using Utility;

    public class MagicDictionary<K,V> : IDictionary<object, object>
    {
        private readonly IDictionary<K, V> _realDictionary;
        private Func<object, K> _typeKeyCaster;

        public MagicDictionary(Object opaqueDictionary)
        {
            _realDictionary = (IDictionary<K, V>)opaqueDictionary;
            _typeKeyCaster = null;
        }

        public MagicDictionary(IDictionary<K, V> realDictionary)
        {
            _realDictionary = realDictionary;
        }

        public Func<object, K> TypeKeyCaster
        {
            get
            {
                if (_typeKeyCaster == null)
                    _typeKeyCaster = Caster.GetCastConverter<K>();
                return _typeKeyCaster;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<object, object>> GetEnumerator()
        {
            foreach( var entry in _realDictionary ) {
                yield return new KeyValuePair<object, object>(entry.Key, entry.Value);
            }
        }

        public void Add(KeyValuePair<object, object> item)
        {
            _realDictionary.Add(new KeyValuePair<K, V>((K) item.Key, (V)item.Value));
        }

        public void Clear()
        {
            _realDictionary.Clear();
        }

        public bool Contains(KeyValuePair<object, object> item)
        {
            return _realDictionary.Contains(new KeyValuePair<K, V>((K) item.Key, (V)item.Value));
        }

        public void CopyTo(KeyValuePair<object, object>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<object, object> item)
        {
            return _realDictionary.Remove(new KeyValuePair<K, V>((K) item.Key, (V)item.Value));
        }

        public int Count
        {
            get { return _realDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _realDictionary.IsReadOnly; }
        }

        public bool ContainsKey(object key)
        {
            if (key is K)
            {
                return _realDictionary.ContainsKey((K) key);
            }
            else
            {
                return _realDictionary.ContainsKey(TypeKeyCaster.Invoke(key));
            }
        }

        public void Add(object key, object value)
        {
            _realDictionary.Add((K) key, (V) value);
        }

        public bool Remove(object key)
        {
            return _realDictionary.Remove((K) key);
        }

        public bool TryGetValue(object key, out object value)
        {
            V item;
            if (_realDictionary.TryGetValue((K) key, out item)) {
                value = item;
                return true;
            }

            value = null;
            return false;
        }

        public object this[object key]
        {
            get { return _realDictionary[(K) key]; }
            set { _realDictionary[(K) key] = (V) value; }
        }

        public ICollection<object> Keys
        {
            get { return new MagicCollection<K>(_realDictionary.Keys); }
        }

        public ICollection<object> Values
        {
            get { return new MagicCollection<V>(_realDictionary.Values); }
        }
    }
}
