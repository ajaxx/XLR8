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
	/// Simple boilerplate for common reader-writer lock implementations
	/// </summary>
	public interface IReaderWriterLockCommon
	{
		/// <summary>
        /// Acquires the reader lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        void AcquireReaderLock(int timeout);

        /// <summary>
        /// Acquires the writer lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        void AcquireWriterLock(int timeout);

        /// <summary>
        /// Releases the reader lock.
        /// </summary>
        void ReleaseReaderLock();

        /// <summary>
        /// Releases the writer lock.
        /// </summary>
        void ReleaseWriterLock();
	}
	
	public interface IReaderWriterLockCommon<T>
	{
		/// <summary>
        /// Acquires the reader lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        T AcquireReaderLock(int timeout);

        /// <summary>
        /// Acquires the writer lock.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        T AcquireWriterLock(int timeout);

        /// <summary>
        /// Releases the reader lock.
        /// </summary>
        void ReleaseReaderLock(T value);

        /// <summary>
        /// Releases the writer lock.
        /// </summary>
        void ReleaseWriterLock(T value);
	}
}
