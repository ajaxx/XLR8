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
    public sealed class StandardReaderWriterLock 
        : IReaderWriterLock
        , IReaderWriterLockCommon
    {
        private readonly ReaderWriterLock _rwLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardReaderWriterLock"/> class.
        /// </summary>
        public StandardReaderWriterLock()
        {
            _rwLock = new ReaderWriterLock();
            ReadLock = new CommonReadLock(this);
            WriteLock = new CommonWriteLock(this);
        }

        /// <summary>
        /// Gets the read-side lockable
        /// </summary>
        /// <value></value>
        public ILockable ReadLock { get ; private set; }

        /// <summary>
        /// Gets the write-side lockable
        /// </summary>
        /// <value></value>
        public ILockable WriteLock { get;  private set; }

#if DEBUG
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StandardReaderWriterLock"/> is trace.
        /// </summary>
        /// <value><c>true</c> if trace; otherwise, <c>false</c>.</value>
        public bool Trace { get; set; }
#endif
        
        /// <summary>
        /// Acquires the reader lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void AcquireReaderLock(int timeout)
        {
            try
            {
                _rwLock.AcquireReaderLock(timeout);
            }
            catch(ApplicationException)
            {
                throw new TimeoutException("ReaderWriterLock timeout expired");
            }
        }

        /// <summary>
        /// Acquires the writer lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void AcquireWriterLock(int timeout)
        {
            try
            {
                _rwLock.AcquireWriterLock(timeout);
            }
            catch(ApplicationException)
            {
                throw new TimeoutException("ReaderWriterLock timeout expired");
            }
        }

        /// <summary>
        /// Releases the reader lock.
        /// </summary>
        public void ReleaseReaderLock()
        {
            _rwLock.ReleaseReaderLock();
        }

        /// <summary>
        /// Releases the writer lock.
        /// </summary>
        public void ReleaseWriterLock()
        {
            _rwLock.ReleaseWriterLock();
        }
    }
}
