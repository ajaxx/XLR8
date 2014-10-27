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

using System.Collections.Generic;

namespace XLR8.Collections
{
    public interface IDeque<T> : ICollection<T>
    {
        void AddLast(T value);
        void AddFirst(T value);

        T RemoveFirst();
        T RemoveLast();

        /// <summary>
        /// Retrieves and removes the head of the queue represented by this deque or returns null if deque is empty.
        /// </summary>
        /// <returns></returns>
        T Poll();

        /// <summary>
        /// Retrieves, but does not remove, the head of the queue represented by this deque, or returns null if this deque is empty.
        /// </summary>
        /// <returns></returns>
        T Peek();

        T First { get; }
        T Last { get; }
    }
}
