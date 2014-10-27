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

namespace XLR8.Threading
{
	/// <summary>
	/// Uses a standard lock to model a reader-writer ... not for general use
	/// </summary>
	public class DummyReaderWriterLock
		: IReaderWriterLock
	{
		/// <summary>
		/// Constructs a new instance of a DummyReaderWriterLock
		/// </summary>
		public DummyReaderWriterLock()
		{
            ReadLock = WriteLock = LockManager.Default.CreateDefaultLock();
		}
		
		/// <summary>
        /// Gets the read-side lockable
        /// </summary>
        public ILockable ReadLock { get; private set; }

        /// <summary>
        /// Gets the write-side lockable
        /// </summary>
        public ILockable WriteLock { get; private set; }

#if DEBUG
        public bool Trace { get; set; }
#endif
	}
}
