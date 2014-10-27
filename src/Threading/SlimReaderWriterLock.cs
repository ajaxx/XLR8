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
    public sealed class SlimReaderWriterLock
    	: IReaderWriterLock
    	, IReaderWriterLockCommon
    {
#if MONO
        public const string ExceptionText = "ReaderWriterLockSlim is not supported on this platform";
#else
        private readonly ReaderWriterLockSlim _rwLock;

#if DIAGNOSTICS
        internal bool IsWriteLocked;
        internal StackTrace WriteTrace;
#endif
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="SlimReaderWriterLock"/> class.
        /// </summary>
        public SlimReaderWriterLock()
        {
#if MONO
            throw new NotSupportedException(ExceptionText);
#else
            _rwLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            ReadLock = new CommonReadLock(this);
            WriteLock = new CommonWriteLock(this);
#endif
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
        /// Gets or sets a value indicating whether this <see cref="SlimReaderWriterLock"/> is trace.
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
#if MONO
            throw new NotSupportedException(ExceptionText);
#else
            if (!_rwLock.TryEnterReadLock(timeout))
            {
                throw new TimeoutException("ReaderWriterLock timeout expired");
            }
#endif
        }

        /// <summary>
        /// Acquires the writer lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        public void AcquireWriterLock(int timeout)
        {
#if MONO
            throw new NotSupportedException(ExceptionText);
#else
            if (!_rwLock.TryEnterWriteLock(timeout))
            {
                throw new TimeoutException("ReaderWriterLock timeout expired");
            }

#if DIAGNOSTICS
            IsWriteLocked = true;
            WriteTrace = new StackTrace();
#endif
#endif
        }

        /// <summary>
        /// Releases the reader lock.
        /// </summary>
        public void ReleaseReaderLock()
        {
#if MONO
            throw new NotSupportedException(ExceptionText);
#else
            _rwLock.ExitReadLock();
#endif
        }

        /// <summary>
        /// Releases the writer lock.
        /// </summary>
        public void ReleaseWriterLock()
        {
#if MONO
            throw new NotSupportedException(ExceptionText);
#else
            _rwLock.ExitWriteLock();

#if DIAGNOSTICS
            IsWriteLocked = false;
            WriteTrace = null;
#endif
#endif
        }
    }
}
