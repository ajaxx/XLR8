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

namespace XLR8.Collections
{
    public interface IBlockingQueue<T>
    {
        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        /// <value>The count.</value>
        int Count { get; }
        /// <summary>
        /// Clears all items from the queue
        /// </summary>
        void Clear();
        /// <summary>
        /// Pushes an item onto the queue.  If the queue has reached
        /// capacity, the call will pend until the queue has space to
        /// receive the request.
        /// </summary>
        /// <param name="item"></param>
        void Push(T item);
        /// <summary>
        /// Pops an item off the queue.  If there is nothing on the queue
        /// the call will pend until there is an item on the queue.
        /// </summary>
        /// <returns></returns>
        T Pop();

        /// <summary>
        /// Pops an item off the queue.  If there is nothing on the queue
        /// the call will pend until there is an item on the queue or
        /// the timeout has expired.  If the timeout has expired, the
        /// method will return false.
        /// </summary>
        /// <param name="maxTimeoutInMillis">The max timeout in millis.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        bool Pop(int maxTimeoutInMillis, out T item);
    }
}
