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
    using Utility;

    /// <summary>
	/// Description of CommonReadLock.
	/// </summary>
	internal sealed class CommonReadLock 
        : ILockable
	{
        private readonly IReaderWriterLockCommon _lockObj;
	    private readonly IDisposable _disposableObj;

        public IDisposable Acquire()
        {
            _lockObj.AcquireReaderLock(BaseLock.RLockTimeout);
            return _disposableObj;
        }

	    public IDisposable Acquire(int msec)
	    {
            _lockObj.AcquireReaderLock(msec);
            return _disposableObj;
        }

        public IDisposable Acquire(bool releaseLock, int? msec = null)
        {
            _lockObj.AcquireReaderLock(msec ?? BaseLock.RLockTimeout);
            if (releaseLock)
                return _disposableObj;
            return new VoidDisposable();
        }

	    public IDisposable ReleaseAcquire()
        {
            _lockObj.ReleaseReaderLock();
            return new TrackedDisposable(() => _lockObj.AcquireReaderLock(BaseLock.RLockTimeout));
        }

	    public void Release()
	    {
	        _lockObj.ReleaseReaderLock();
	    }

	    internal CommonReadLock(IReaderWriterLockCommon lockObj)
        {
            _lockObj = lockObj;
            _disposableObj = new TrackedDisposable(_lockObj.ReleaseReaderLock);
        }
	}
	
	/// <summary>
	/// Description of CommonReadLock.
	/// </summary>
    internal sealed class CommonReadLock<T> : ILockable
	{
        private readonly IReaderWriterLockCommon<T> _lockObj;
        private T _lockValue;

        public IDisposable Acquire()
        {
            _lockValue = _lockObj.AcquireReaderLock(BaseLock.RLockTimeout);
            return new TrackedDisposable(() => _lockObj.ReleaseReaderLock(_lockValue));
        }

        public IDisposable Acquire(int msec)
        {
            _lockValue = _lockObj.AcquireReaderLock(msec);
            return new TrackedDisposable(() => _lockObj.ReleaseReaderLock(_lockValue));
        }

	    public IDisposable Acquire(bool releaseLock, int? msec = null)
	    {
            _lockValue = _lockObj.AcquireReaderLock(msec ?? BaseLock.RLockTimeout);
            if (releaseLock)
                return new TrackedDisposable(() => _lockObj.ReleaseReaderLock(_lockValue));
	        return new VoidDisposable();
	    }

	    public IDisposable ReleaseAcquire()
        {
            _lockObj.ReleaseReaderLock(_lockValue);
            _lockValue = default(T);
            return new TrackedDisposable(() => _lockValue = _lockObj.AcquireReaderLock(BaseLock.RLockTimeout));
        }

        public void Release()
        {
            _lockObj.ReleaseReaderLock(_lockValue);
            _lockValue = default(T);
        }

        internal CommonReadLock(IReaderWriterLockCommon<T> lockObj)
        {
            _lockObj = lockObj;
        }
	}
}
