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
    /// Base class for disposable lock pattern.
    /// </summary>
    abstract public class BaseLock : IDisposable
    {
        /// <summary>
        /// The default read lock timeout
        /// </summary>
        public static int RLockTimeout;
        /// <summary>
        /// The default write lock timeout
        /// </summary>
        public static int WLockTimeout;
        /// <summary>
        /// The default monitor lock timeout
        /// </summary>
        public static int MLockTimeout;

        /// <summary>
        /// Initializes the <see cref="BaseLock"/> class.
        /// </summary>
        static BaseLock()
        {
            RLockTimeout = 60000;
            WLockTimeout = 60000;
            MLockTimeout = 60000;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public abstract void Dispose();
    }
}
