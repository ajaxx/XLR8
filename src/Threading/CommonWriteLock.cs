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
	/// Description of CommonWriteLock.
	/// </summary>
	internal class CommonWriteLock : ILockable
	{
        private readonly IReaderWriterLockCommon _lockObj;

        public IDisposable Acquire()
        {
            _lockObj.AcquireWriterLock(BaseLock.WLockTimeout);
            return new TrackedDisposable(() => _lockObj.ReleaseWriterLock());
        }

	    public IDisposable Acquire(int msec)
	    {
            _lockObj.AcquireWriterLock(msec);
            return new TrackedDisposable(() => _lockObj.ReleaseWriterLock());
        }

	    public IDisposable Acquire(bool releaseLock, int? msec = null)
	    {
            _lockObj.AcquireWriterLock(msec ?? BaseLock.WLockTimeout);
            if (releaseLock)
                return new TrackedDisposable(() => _lockObj.ReleaseWriterLock());
            return new VoidDisposable();
	    }

	    public IDisposable ReleaseAcquire()
        {
            _lockObj.ReleaseWriterLock();
            return new TrackedDisposable(() => _lockObj.AcquireWriterLock(BaseLock.RLockTimeout));
        }

        public void Release()
        {
            _lockObj.ReleaseWriterLock();
        }

        internal CommonWriteLock(IReaderWriterLockCommon lockObj)
        {
            _lockObj = lockObj;
        }
	}
	
	/// <summary>
	/// Description of CommonWriteLock.
	/// </summary>
	internal class CommonWriteLock<T> : ILockable
	{
        private readonly IReaderWriterLockCommon<T> _lockObj;
        private T _lockValue;

        public IDisposable Acquire()
        {
            _lockValue = _lockObj.AcquireWriterLock(BaseLock.WLockTimeout);
            return new TrackedDisposable(() => _lockObj.ReleaseWriterLock(_lockValue));
        }

	    public IDisposable Acquire(int msec)
	    {
            _lockValue = _lockObj.AcquireWriterLock(msec);
            return new TrackedDisposable(() => _lockObj.ReleaseWriterLock(_lockValue));
        }

        public IDisposable Acquire(bool releaseLock, int? msec = null)
        {
            _lockValue = _lockObj.AcquireWriterLock(msec ?? BaseLock.WLockTimeout);
            if (releaseLock)
                return new TrackedDisposable(() => _lockObj.ReleaseWriterLock(_lockValue));
            return new VoidDisposable();
        }

	    public IDisposable ReleaseAcquire()
        {
            _lockObj.ReleaseWriterLock(_lockValue);
            _lockValue = default(T);
            return new TrackedDisposable(() => _lockValue = _lockObj.AcquireWriterLock(BaseLock.RLockTimeout));
        }

        public void Release()
        {
            _lockObj.ReleaseWriterLock(_lockValue);
            _lockValue = default(T);
        }

        internal CommonWriteLock(IReaderWriterLockCommon<T> lockObj)
        {
            _lockObj = lockObj;
        }
	}
}
