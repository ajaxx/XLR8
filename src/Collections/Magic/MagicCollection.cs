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
using System.Linq;

namespace XLR8.Collections.Magic
{
    using Utility;

    public class MagicCollection<V> : ICollection<Object>
    {
        private readonly ICollection<V> _realCollection;
        private Func<object, V> _typeCaster;

        public MagicCollection(object opaqueCollection)
        {
            _realCollection = (ICollection<V>) opaqueCollection;
            _typeCaster = null;
        }

        public MagicCollection(ICollection<V> realCollection)
        {
            _realCollection = realCollection;
        }

        public Func<object, V> TypeCaster
        {
            get
            {
                if (_typeCaster == null)
                    _typeCaster = Caster.GetCastConverter<V>();
                return _typeCaster;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return _realCollection.GetEnumerator();
        }

        IEnumerator<object> IEnumerable<object>.GetEnumerator()
        {
            foreach (V item in _realCollection )
                yield return item;
        }

        public void Add(object item)
        {
            _realCollection.Add((V)item);
        }

        public void Clear()
        {
            _realCollection.Clear();
        }

        public bool Contains(object item)
        {
            if (item is V) {
                return _realCollection.Contains((V) item);
            } else {
                return _realCollection.Contains(TypeCaster.Invoke(item));
            }
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            foreach(var item in this)
            {
                if (arrayIndex >= array.Length)
                {
                    break;
                }

                array[arrayIndex++] = item;
            }
        }

        public bool Remove(object item)
        {
            if (item is V) {
                return _realCollection.Remove((V) item);
            } else {
                return _realCollection.Remove(TypeCaster.Invoke(item));
            }
        }

        public int Count
        {
            get { return _realCollection.Count(); }
        }

        public bool IsReadOnly
        {
            get { return _realCollection.IsReadOnly; }
        }
    }
}
