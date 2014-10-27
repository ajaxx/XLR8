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
    /// <summary>
    /// A simple locking mechanism
    /// </summary>

    public interface ILockable
    {
        /// <summary>
        /// Acquires the lock; the lock is released when the disposable
        /// object that was returned is disposed.
        /// </summary>
        /// <returns></returns>
        IDisposable Acquire();

        /// <summary>
        /// Acquire the lock; the lock is released when the disposable
        /// object that was returned is disposed IF the releaseLock
        /// flag is set.
        /// </summary>
        /// <param name="releaseLock"></param>
        /// <param name="msec"></param>
        /// <returns></returns>
        IDisposable Acquire(bool releaseLock, int? msec = null);

        /// <summary>
        /// Acquires the specified msec.
        /// </summary>
        /// <param name="msec">The msec.</param>
        /// <returns></returns>
        IDisposable Acquire(int msec);

        /// <summary>
        /// Provides a temporary release of the lock if it is acquired.  When the
        /// disposable object that is returned is disposed, the lock is re-acquired.
        /// This method is effectively the opposite of acquire.
        /// </summary>
        /// <returns></returns>
        IDisposable ReleaseAcquire();

        /// <summary>
        /// Releases this instance.
        /// </summary>
        void Release();
    }
}
