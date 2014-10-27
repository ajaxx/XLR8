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
    /// <summary>
    /// IThreadLocal implementation that uses the native support
    /// in the CLR (i.e. the LocalDataStoreSlot).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SystemThreadLocal<T> : IThreadLocal<T>
        where T : class
    {
        /// <summary>
        /// Local data storage slot
        /// </summary>
        private readonly LocalDataStoreSlot _dataStoreSlot;

        /// <summary>
        /// Factory delegate for construction of data on miss.
        /// </summary>

        private readonly Func<T> _dataFactory;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return (T) Thread.GetData(_dataStoreSlot); }
            set { Thread.SetData(_dataStoreSlot, value); }
        }

        /// <summary>
        /// Gets the data or creates it if not found.
        /// </summary>
        /// <returns></returns>
        public T GetOrCreate()
        {
            T value = (T)Thread.GetData(_dataStoreSlot);
            if ( value == null )
            {
                value = _dataFactory();
                Thread.SetData( _dataStoreSlot, value );
            }

            return value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemThreadLocal&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="factory">The factory used to create values when not found.</param>
        public SystemThreadLocal( Func<T> factory )
        {
            _dataStoreSlot = Thread.AllocateDataSlot();
            _dataFactory = factory;
        }
    }

    /// <summary>
    /// Creates system thread local objects.
    /// </summary>
    public class SystemThreadLocalFactory : ThreadLocalFactory
    {
        #region ThreadLocalFactory Members

        /// <summary>
        /// Create a thread local object of the specified type param.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IThreadLocal<T> CreateThreadLocal<T>(Func<T> factory) where T : class
        {
            return new SystemThreadLocal<T>(factory);
        }

        #endregion
    }
}
