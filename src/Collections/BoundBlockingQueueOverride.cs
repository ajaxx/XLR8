﻿// --------------------------------------------------------------------------------
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

namespace XLR8.Collections
{
    using Utility;

    internal sealed class BoundBlockingQueueOverride
    {
        internal static BoundBlockingQueueOverride Default;

        /// <summary>
        /// Gets a value indicating whether the override is engaged.
        /// </summary>
        /// <value><c>true</c> if the override is engaged; otherwise, <c>false</c>.</value>
        internal static bool IsEngaged
        {
            get { return ScopedInstance<BoundBlockingQueueOverride>.Current != null; }
        }

        /// <summary>
        /// Initializes the <see cref="BoundBlockingQueueOverride"/> class.
        /// </summary>
        static BoundBlockingQueueOverride()
        {
            Default = new BoundBlockingQueueOverride();
        }
    }
}
