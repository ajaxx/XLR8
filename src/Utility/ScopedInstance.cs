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
    /// <summary>
    /// Provides a generic item that can be scoped statically as a singleton; avoids the
    /// need to define a threadstatic variable.  Also provides a consistent model for
    /// providing this service.
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class ScopedInstance<T> where T : class
    {
        [ThreadStatic]
        private static T _instance;

        /// <summary>
        /// Gets the current instance value.
        /// </summary>
        /// <value>The current.</value>
        public static T Current
        {
            get { return _instance; }
        }

        public static bool IsSet
        {
            get { return _instance != default(T); }
        }

        /// <summary>
        /// Sets the specified instance.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IDisposable Set(T item)
        {
            return new DisposableScope(item);
        }

        /// <summary>
        /// Disposable scope
        /// </summary>
        private class DisposableScope : IDisposable
        {
            private readonly T _previous;

            /// <summary>
            /// Initializes a new instance of the <see cref="ScopedInstance&lt;T&gt;.DisposableScope"/> class.
            /// </summary>
            /// <param name="item">The item.</param>
            internal DisposableScope(T item)
            {
                _previous = _instance;
                _instance = item;
            }

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _instance = _previous;
            }

            #endregion
        }
    }
}
