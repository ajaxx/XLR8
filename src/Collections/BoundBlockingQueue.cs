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
using System.Threading;

namespace XLR8.Collections
{
    using Threading;
    using Utility;

    public class BoundBlockingQueue<T> : IBlockingQueue<T>
    {
        private readonly LinkedList<T> _queue;
        private readonly ILockable _queueLock;
        private readonly Object _queuePopWaitHandle;
        private readonly Object _queuePushWaitHandle;
        private readonly int _maxCapacity;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundBlockingQueue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="maxCapacity">The max capacity.</param>
        public BoundBlockingQueue(int maxCapacity)
        {
            _queue = new LinkedList<T>();
            _queueLock = new MonitorSpinLock();
            _queuePopWaitHandle = new AutoResetEvent(false);
            _queuePushWaitHandle = new AutoResetEvent(false);
            _maxCapacity = maxCapacity;
        }

        /// <summary>
        /// Gets the number of items in the queue.
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                using (_queueLock.Acquire())
                {
                    return _queue.Count;
                }
            }
        }

        /// <summary>
        /// Clears all items from the queue
        /// </summary>
        public void Clear()
        {
            using (_queueLock.Acquire())
            {
                _queue.Clear();

                PulsePushHandle();
                PulsePopHandle();

                //_queuePushWaitHandle.Set();  // Push is clear
                //_queuePopWaitHandle.Reset(); // Pop now waits
            }
        }

        /// <summary>
        /// Pushes an item onto the queue.  If the queue has reached
        /// capacity, the call will pend until the queue has space to
        /// receive the request.
        /// </summary>
        /// <param name="item"></param>
        public void Push(T item)
        {
            while (true)
            {
                using (_queueLock.Acquire()) {
                    if ((_queue.Count < _maxCapacity) || BoundBlockingQueueOverride.IsEngaged)
                    {
                        _queue.AddLast(item);
                        PulsePopHandle();
                        return;
                    }
                }

                WaitPushHandle();

                //_queuePushWaitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Pops an item off the queue.  If there is nothing on the queue
        /// the call will pend until there is an item on the queue.
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            while (true)
            {
                using (_queueLock.Acquire())
                {
                    var first = _queue.First;
                    if (first != null)
                    {
                        var value = first.Value;
                        _queue.RemoveFirst();

                        PulsePushHandle();

                        //_queuePushWaitHandle.Set(); // Push is clear
                        return value;
                    }
                }

                WaitPopHandle();

                //_queuePopWaitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Pops an item off the queue.  If there is nothing on the queue
        /// the call will pend until there is an item on the queue or
        /// the timeout has expired.  If the timeout has expired, the
        /// method will return false.
        /// </summary>
        /// <param name="maxTimeoutInMillis">The max timeout in millis.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Pop(int maxTimeoutInMillis, out T item)
        {
            long endTime = DateTimeExtensions.CurrentTimeMillis + maxTimeoutInMillis;

            do {
                using (_queueLock.Acquire()) {
                    var first = _queue.First;
                    if (first != null) {
                        var value = first.Value;
                        _queue.RemoveFirst();

                        PulsePushHandle();

                        item = value;
                        return true;
                    }
                }

                var nowTime = DateTimeExtensions.CurrentTimeMillis;
                if (nowTime >= endTime) {
                    item = default(T);
                    return false;
                }

                WaitPopHandle((int) (endTime - nowTime));
            } while (true);
        }

        private void PulsePushHandle()
        {
            Monitor.Enter(_queuePushWaitHandle);
            Monitor.PulseAll(_queuePushWaitHandle);
            Monitor.Exit(_queuePushWaitHandle);
        }

        private void PulsePopHandle()
        {
            Monitor.Enter(_queuePopWaitHandle);
            Monitor.PulseAll(_queuePopWaitHandle);
            Monitor.Exit(_queuePopWaitHandle);
        }

        private void WaitPushHandle()
        {
            Monitor.Enter(_queuePushWaitHandle);
            Monitor.Wait(_queuePushWaitHandle);
            Monitor.Exit(_queuePushWaitHandle);
        }

        private void WaitPopHandle()
        {
            Monitor.Enter(_queuePopWaitHandle);
            Monitor.Wait(_queuePopWaitHandle);
            Monitor.Exit(_queuePopWaitHandle);
        }

        private void WaitPopHandle(int timeToWait)
        {
            Monitor.Enter(_queuePopWaitHandle);
            Monitor.Wait(_queuePopWaitHandle, timeToWait);
            Monitor.Exit(_queuePopWaitHandle);
        }
    }
}
