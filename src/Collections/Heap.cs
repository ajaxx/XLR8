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
using System.Text;

namespace XLR8.Collections
{
    public class Heap<TK> : ICollection<TK>
        where TK : IComparable<TK>
    {
        /// <summary>
        /// The root of the heap
        /// </summary>
        private TK[] _data;

        /// <summary>
        /// The number of entries the heap.
        /// </summary>
        private int _count;

        /// <summary>
        /// The initial capacity
        /// </summary>
        private int _initialCapacity;

        /// <summary>
        /// Used to compare keys.
        /// </summary>
        private readonly Comparison<TK> _comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Heap{TK}" /> class.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity.</param>
        public Heap(int initialCapacity = 128)
        {
            _initialCapacity = initialCapacity;
            _data = new TK[initialCapacity];
            _count = 0;
            _comparer = DefaultComparer;
        }

        /// <summary>
        /// Gets the maximum.
        /// </summary>
        /// <value>
        /// The maximum.
        /// </value>
        /// <exception cref="System.InvalidOperationException"></exception>
        public TK Max
        {
            get
            {
                if (_count == 0)
                {
                    throw new InvalidOperationException();
                }

                return _data[0];
            }
        }

        /// <summary>
        /// Gets the length of the heap.
        /// </summary>
        /// <value>
        /// The length of the heap.
        /// </value>
        public int HeapLength
        {
            get { return _data.Length; }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public int Count
        {
            get { return _count; }
            set { _count = value; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Finds the specified item.  Returns the index in the heap.  Finding an item in a heap
        /// is an O(n) operation.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public int Find(TK item)
        {
            var count = _count;
            for (int ii = 0; ii < count; ii++)
            {
                if (_comparer.Invoke(_data[ii], item) == 0)
                {
                    return ii;
                }
            }

            return -1;
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CopyTo(TK[] array, int arrayIndex)
        {
            Array.Copy(_data, 0, array, 0, _count);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(TK item)
        {
            return Find(item) != -1;
        }

        /// <summary>
        /// Adds a range of items
        /// </summary>
        /// <param name="items">The more items.</param>
        public void Add(params TK[] items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void Add(TK item)
        {
            var index = _count;
            if (index >= _data.Length) // array does not have space for growth
            {
                var array = new TK[_data.Length << 1];
                Array.Copy(_data, 0, array, 0, _data.Length);
                _data = array;
            }

            // add the data to the rightmost position of the data in the heap
            _data[index] = item;
            _count++;

            // sift up
            while (index > 0)
            {
                var parent = (index - 1) / 2;
                var result = _comparer.Invoke(_data[index], _data[parent]);
                if (result > 0)
                {
                    var cvalue = _data[index];
                    _data[index] = _data[parent];
                    _data[parent] = cvalue;
                    index = parent;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool Remove(TK item)
        {
            var index = Find(item);
            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Removes the item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public void RemoveAt(int index)
        {
            if ((index < 0) || (index >= _count))
            {
                throw new IndexOutOfRangeException();
            }

            var eindex = _count - 1;
            if (eindex == index)
            {
                _data[eindex] = default(TK);
                _count = 0;
                return;
            }

            _data[index] = _data[eindex];
            _data[eindex] = default(TK);
            _count--;

            while (true)
            {
                var childA = index * 2 + 1;
                if (childA >= _count) // child index past end of heap
                {
                    break;
                }

                var childB = childA + 1;

                // is the heap violated at this level?
                var resultA = _comparer.Invoke(_data[index], _data[childA]);
                var resultB = _comparer.Invoke(_data[index], _data[childB]);
                if ((resultA < 0) && (resultB < 0)) // violation to both children
                {
                    var result = _comparer.Invoke(_data[childA], _data[childB]);
                    if (result > 0) // childA is "larger"
                    {
                        index = SiftDown(index, childA);
                    }
                    else // child is larger
                    {
                        index = SiftDown(index, childB);
                    }
                }
                else if (resultA < 0) // violation to heap A
                {
                    index = SiftDown(index, childA);
                }
                else if (resultB < 0) // violation to heap B
                {
                    index = SiftDown(index, childB);
                }
                else // no heap violation
                {
                    break;
                }
            }
        }

        private int SiftDown(int index, int child)
        {
            var value = _data[index];
            _data[index] = _data[child];
            _data[child] = value;
            index = child;
            return index;
        }

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        public void Clear()
        {
            _data = new TK[_initialCapacity];
            _count = 0;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerator<TK> GetEnumerator()
        {
            for (int ii = 0; ii < _count; ii++)
            {
                yield return _data[ii];
            }
        }

        /// <summary>
        /// Gets an array form of the data.
        /// </summary>
        /// <returns></returns>
        public TK[] GetArray()
        {
            var array = new TK[_count];
            Array.Copy(_data, 0, array, 0, _count);
            return array;
        }

        /// <summary>
        /// Gets the shape.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        internal StringBuilder AddShape(StringBuilder builder, int index)
        {
            var self = Convert.ToString(_data[index]);
            builder.Append(self);

            var childA = index * 2 + 1;
            if (childA < _count)
            {
                var childB = childA + 1;

                builder.Append('{');
                AddShape(builder, childA);
                if (childB < _count)
                {
                    builder.Append(',');
                    AddShape(builder, childB);
                }

                builder.Append('}');
            }

            return builder;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (_count != 0)
            {
                return AddShape(new StringBuilder(), 0).ToString();
            }

            return string.Empty;
        }

        #region DefaultComparer

        public static int DefaultComparer(TK x, TK y)
        {
            return x.CompareTo(y);
        }

        #endregion

        internal class Node
        {
            internal TK Value;
            internal Node Left;
            internal Node Right;
            internal int Height;
        }
    }
}
