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
using System.Threading;

namespace XLR8.Threading
{
    using Utility;

    public class CountDownLatch
    {
        private long _latchCount;

        public CountDownLatch(long latchCount)
        {
            _latchCount = latchCount;
        }

        /// <summary>
        /// Returns the number of outstanding latches that have not been
        /// removed.
        /// </summary>
        /// <value>The count.</value>
        public long Count
        {
            get { return Interlocked.Read(ref _latchCount); }
        }

        public void CountDown()
        {
            if (Interlocked.Decrement(ref _latchCount) == 0)
            {
                
            }
        }

        /// <summary>
        /// Waits for the latch to be released for up to the specified amount of time.
        /// If the timeout expires a TimeoutException is thrown.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        public bool Await(TimeSpan timeout)
        {
            var timeCur = DateTimeExtensions.CurrentTimeMillis;
            var timeEnd = timeCur + (long) timeout.TotalMilliseconds;
            var iteration = 0;

            while(Interlocked.Read(ref _latchCount) > 0)
            {
                if (!SlimLock.SmartWait(++iteration, timeEnd))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Awaits this instance.
        /// </summary>
        /// <returns></returns>
        public bool Await()
        {
            return Await(TimeSpan.MaxValue);
        }
    }
}
