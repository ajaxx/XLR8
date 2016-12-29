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

namespace XLR8.Collections
{
    [Serializable]
    public class ArrayDeque<T> : IDeque<T>
    {
        private int _head;
        private int _tail;
        private T[] _array;

        public ArrayDeque()
        {
            _head = 0;
            _tail = 0;
            _array = new T[128];
        }

        public ArrayDeque(int capacity)
        {
            _head = 0;
            _tail = 0;
            _array = new T[capacity];
        }

        public ArrayDeque(ICollection<T> source)
        {
            var ncount = source.Count;
            var lcount = (int) Math.Log(ncount, 2);
            var mcount = 1 << lcount;
            if (mcount <= ncount)
                mcount <<= 1;

            _array = new T[mcount];
            source.CopyTo(_array, 0);
            _head = 0;
            _tail = ncount;
        }
            
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_tail == _head)
            {
            }
            else if (_tail > _head)
            {
                for (int ii = _head; ii < _tail; ii++)
                    yield return _array[ii];
            }
            else
            {
                for (int ii = _head; ii < _array.Length; ii++)
                    yield return _array[ii];
                for (int ii = 0; ii < _tail; ii++)
                    yield return _array[ii];
            }
        }

        private void DoubleCapacity()
        {
            int newLength = _array.Length << 1;
            if (newLength < 0)
                throw new InvalidOperationException("ArrayDeque overflow");

            var narray = new T[newLength];

            if (_tail > _head)
            {
                Array.Copy(_array, _head, narray, 0, _tail - _head);
            }
            else
            {
                var nl = _head;
                var nr = _array.Length - _head;

                Array.Copy(_array, _head, narray, 0, nr);
                Array.Copy(_array, 0, narray, nr, nl);
            }

            _head = 0;
            _tail = _array.Length;
            _array = narray;
        }

        public void AddFirst(T item)
        {
            if (--_head == -1)
                _head = _array.Length - 1;
            if (_head == _tail)
                DoubleCapacity();
            _array[_head] = item;
        }

        public void AddLast(T item)
        {
            _array[_tail] = item;
            if (++_tail == _array.Length)
                _tail = 0;
            if (_head == _tail)
                DoubleCapacity();
        }

        public void Add(T item)
        {
            AddLast(item);
        }

        public T RemoveFirst()
        {
            if (_head == _tail)
                throw new InvalidOperationException();
            // preserve the value at the head
            var result = _array[_head];
            // clear the value in the array
            _array[_head] = default(T);
            // increment the head
            if (++_head == _array.Length)
                _head = 0;

            return result;
        }

        public T RemoveLast()
        {
            if (_tail == _head)
                throw new InvalidOperationException();
            if (--_tail < 0)
                _tail = _array.Length - 1;
            var result = _array[_tail];
            _array[_tail] = default(T);
            return result;
        }

        public TM RemoveInternal<TM>(int index, TM returnValue)
        {
            if (_tail > _head)
            {
                int tindex = _head + index;
                if (tindex >= _tail)
                    throw new ArgumentOutOfRangeException();
                for (int ii = tindex + 1 ; ii < _tail ; ii++)
                    _array[ii - 1] = _array[ii];
                _array[--_tail] = default(T);
            }
            else if (index > (_tail + _array.Length - _head))
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                int tindex = (_head + index)%_array.Length;
                if (tindex > _head)
                {
                    for (int ii = tindex + 1 ; ii < _array.Length ; ii++)
                        _array[ii - 1] = _array[ii];
                    _array[_array.Length - 1] = _array[0];
                    for (int ii = 1 ; ii < _tail ; ii++)
                        _array[ii - 1] = _array[ii];
                    _array[--_tail] = default(T);
                }
                else
                {
                    for (int ii = 1 ; ii < _tail ; ii++)
                        _array[ii - 1] = _array[ii];
                    _array[--_tail] = default(T);
                }

                if (_tail == -1)
                    _tail = _array.Length - 1;
            }

            return returnValue;
        }

        public void RemoveAt(int index)
        {
            RemoveInternal(index, 0);
        }

        public void RemoveWhere(Func<T, bool> predicate, Action<T> onRemoveItem = null)
        {
            T value;

            if (_tail > _head)
            {
                for (int ii = _head; ii < _tail; ii++)
                    if (predicate.Invoke(value = _array[ii]))
                        if (RemoveInternal(ii--, true) && (onRemoveItem != null))
                            onRemoveItem(value);
            }
            else
            {
                for (int ii = _head; ii < _array.Length; ii++)
                    if (predicate.Invoke(value = _array[ii]))
                        if (RemoveInternal(ii--, true) && (onRemoveItem != null))
                            onRemoveItem(value);
                for (int ii = 0; ii < _tail; ii++)
                    if (predicate.Invoke(value = _array[ii]))
                        if (RemoveInternal(ii--, true) && (onRemoveItem != null))
                            onRemoveItem(value);
            }
        }


        public bool Remove(T item)
        {
            if (_tail > _head)
            {
                for (int ii = _head; ii < _tail; ii++)
                    if (Equals(_array[ii], item))
                        return RemoveInternal(ii, true);
            }
            else
            {
                for (int ii = _head; ii < _array.Length; ii++)
                    if (Equals(_array[ii], item))
                        return RemoveInternal(ii, true);
                for (int ii = 0; ii < _tail; ii++)
                    if (Equals(_array[ii], item))
                        return RemoveInternal(ii, true);
            }

            return false;
        }

        public void Clear()
        {
            _head = 0;
            _tail = 0;
            _array.Fill(default(T));
        }

        public bool Contains(T item)
        {
            if (_tail > _head)
            {
                for (int ii = _head; ii < _tail; ii++)
                    if (Equals(_array[ii], item))
                        return true;
            }
            else
            {
                for (int ii = _head; ii < _array.Length; ii++)
                    if (Equals(_array[ii], item))
                        return true;
                for (int ii = 0; ii < _tail; ii++)
                    if (Equals(_array[ii], item))
                        return true;
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (_tail > _head)
            {
                Array.Copy(_array, _head, array, 0, _tail - _head);
            }
            else
            {
                var nl = _head;
                var nr = _array.Length - _head;

                Array.Copy(_array, _head, array, 0, nr);
                Array.Copy(_array, 0, array, nr, nl);
            }
        }

        public T this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException();
                if (_tail > _head)
                    return _array[_head + index];
                if (index >= _tail + _array.Length - _head)
                    throw new ArgumentOutOfRangeException();

                var offset = _head + index;
                if (offset < _array.Length)
                    return _array[offset];

                return _array[offset%_array.Length];
            }

            set
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException();
                if (_tail > _head)
                {
                    _array[_head + index] = value;
                    return;
                }

                if (index >= _tail + _array.Length - _head)
                    throw new ArgumentOutOfRangeException();

                var offset = _head + index;
                if (offset < _array.Length)
                {
                    _array[offset] = value;
                }

                _array[offset%_array.Length] = value;
            }
        }

        /// <summary>
        /// Retrieves and removes the head of the queue represented by this deque or returns null if deque is empty.
        /// </summary>
        /// <returns></returns>
        public T Poll()
        {
            return RemoveFirst();
        }

        public T PopFront()
        {
            return RemoveFirst();
        }

        public T PopBack()
        {
            return RemoveLast();
        }

        /// <summary>
        /// Retrieves, but does not remove, the head of the queue represented by this deque, or returns null if this deque is empty.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return First;
        }

        public T First
        {
            get
            {
                if (_head == _tail)
                    throw new ArgumentOutOfRangeException();
                int indx = _head;
                if (indx == _array.Length)
                    indx = 0;

                return _array[indx];
            }
        }

        public T Last
        {
            get
            {
                if (_head == _tail)
                    throw new ArgumentOutOfRangeException();
                int indx = _tail - 1;
                if (indx == -1)
                    indx = _array.Length - 1;

                return _array[indx];
            }
        }

        public int Count
        {
            get
            {
                if (_tail == _head)
                    return 0;
                if (_tail > _head)
                    return _tail - _head;
                return _tail + _array.Length - _head;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
    }
}
