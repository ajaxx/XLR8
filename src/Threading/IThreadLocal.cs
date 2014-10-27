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
    /// IThreadLocal provides the engine with a way to store information that
    /// is local to the instance and a the thread.  While the CLR provides the
    /// ThreadStatic attribute, it can only be applied to static variables;
    /// some usage patterns (such as statement-specific thread-specific
    /// processing data) require that data be associated by instance and thread.
    /// The CLR provides a solution to this known as LocalDataStoreSlot.  It
    /// has been documented that this method is slower than its ThreadStatic
    /// counterpart, but it allows for instance-based allocation.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public interface IThreadLocal<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        T Value
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the data or creates it if not found.
        /// </summary>
        /// <returns></returns>
        T GetOrCreate();
    }
}
