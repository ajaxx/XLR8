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

    public class MagicList<V> : IList<Object>
    {
        private readonly IList<V> _realList;
        private Func<object, V> _typeCaster;

        public MagicList(object opaqueCollection)
        {
            _realList = (IList<V>)opaqueCollection;
            _typeCaster = null;
        }

        public MagicList(IList<V> realCollection)
        {
            _realList = realCollection;
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

        #region Implementation of IList<object>

        public int IndexOf(object item)
        {
            if (item is V)
            {
                return _realList.IndexOf((V)item);
            }
            else
            {
                return _realList.IndexOf(_typeCaster.Invoke(item));
            }
        }

        public void Insert(int index, object item)
        {
            if (item is V)
            {
                _realList.Insert(index, (V) item);
            }
            else
            {
                _realList.Insert(index, _typeCaster.Invoke(item));
            }
        }

        public void RemoveAt(int index)
        {
            _realList.RemoveAt(index);
        }

        public object this[int index]
        {
            get { return _realList[index]; }
            set
            {
                if (value is V) {
                    _realList[index] = (V) value;
                }
                else {
                    _realList[index] = TypeCaster.Invoke(value);
                }
            }
        }

        #endregion

        public IEnumerator GetEnumerator()
        {
            return _realList.GetEnumerator();
        }

        IEnumerator<Object> IEnumerable<Object>.GetEnumerator()
        {
            foreach (V item in _realList )
                yield return item;
        }

        public void Add(object item)
        {
            _realList.Add((V)item);
        }

        public void Clear()
        {
            _realList.Clear();
        }

        public bool Contains(object item)
        {
            if (item is V) {
                return _realList.Contains((V) item);
            } else {
                return _realList.Contains(TypeCaster.Invoke(item));
            }
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(object item)
        {
            if (item is V) {
                return _realList.Remove((V) item);
            } else {
                return _realList.Remove(TypeCaster.Invoke(item));
            }
        }

        public int Count
        {
            get { return _realList.Count(); }
        }

        public bool IsReadOnly
        {
            get { return _realList.IsReadOnly; }
        }


    }
}
