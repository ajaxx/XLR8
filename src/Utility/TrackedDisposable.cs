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

namespace XLR8.Utility
{
    public sealed class TrackedDisposable : IDisposable
    {
        private readonly Action _actionOnDispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedDisposable"/> class.
        /// </summary>
        /// <param name="actionOnDispose">The action on dispose.</param>
        public TrackedDisposable(Action actionOnDispose)
        {
            _actionOnDispose = actionOnDispose;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _actionOnDispose.Invoke();
        }
    }
}
