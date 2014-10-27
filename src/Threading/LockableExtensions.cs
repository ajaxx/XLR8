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

namespace XLR8.Threading
{
    public static class LockableExtensions
    {
        /// <summary>
        /// Executes an observable call within the scope of the lock.
        /// </summary>
        /// <param name="lockable">The lockable.</param>
        /// <param name="observableCall">The observable call.</param>
        public static void Call(this ILockable lockable, Action observableCall)
        {
            using(lockable.Acquire()) {
                observableCall.Invoke();
            }
        }

        /// <summary>
        /// Executes a function within the scope of the lock.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lockable">The lockable.</param>
        /// <param name="function">The function.</param>
        /// <returns></returns>
        public static T Call<T>(this ILockable lockable, Func<T> function)
        {
            using (lockable.Acquire()) {
                return function.Invoke();
            }
        }
    }
}
