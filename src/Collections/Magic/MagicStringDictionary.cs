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
    public class MagicStringDictionary<V> : IDictionary<string, object>
    {
        private readonly IDictionary<string, V> _realDictionary;

        public MagicStringDictionary(Object opaqueDictionary)
        {
            _realDictionary = (IDictionary<string, V>) opaqueDictionary;
        }

        public MagicStringDictionary(IDictionary<string, V> realDictionary)
        {
            _realDictionary = realDictionary;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach( var entry in _realDictionary ) {
                yield return new KeyValuePair<string, object>(entry.Key, entry.Value);
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _realDictionary.Add(new KeyValuePair<string, V>(item.Key, (V) item.Value));
        }

        public void Clear()
        {
            _realDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _realDictionary.Contains(new KeyValuePair<string, V>(item.Key, (V) item.Value));
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _realDictionary.Remove(new KeyValuePair<string, V>(item.Key, (V) item.Value));
        }

        public int Count
        {
            get { return _realDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _realDictionary.IsReadOnly; }
        }

        public bool ContainsKey(string key)
        {
            return _realDictionary.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            _realDictionary.Add(key, (V) value);
        }

        public bool Remove(string key)
        {
            return _realDictionary.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            V item;
            if (_realDictionary.TryGetValue(key, out item)) {
                value = item;
                return true;
            }

            value = null;
            return false;
        }

        public object this[string key]
        {
            get { return _realDictionary[key]; }
            set { _realDictionary[key] = (V) value; }
        }

        public ICollection<string> Keys
        {
            get { return _realDictionary.Keys; }
        }

        public ICollection<object> Values
        {
            get { return new MagicCollection<V>(_realDictionary.Values); }
        }
    }
}
