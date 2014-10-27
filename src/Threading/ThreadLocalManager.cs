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
    public class ThreadLocalManager
    {
        /// <summary>
        /// Gets or sets the default thread local factory.
        /// </summary>
        /// <value>
        /// The default thread local factory.
        /// </value>
        public ThreadLocalFactory DefaultThreadLocalFactory { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadLocalManager"/> class.
        /// </summary>
        /// <param name="threadLocalFactory">The thread local factory.</param>
        public ThreadLocalManager(ThreadLocalFactory threadLocalFactory = null)
        {
            if (threadLocalFactory == null)
            {
                threadLocalFactory = new FastThreadLocalFactory();
            }

            DefaultThreadLocalFactory = threadLocalFactory;
        }

        /// <summary>
        /// Creates a thread local instance.
        /// </summary>
        /// <returns></returns>
        public IThreadLocal<T> Create<T>(Func<T> factoryDelegate)
            where T : class
        {
            return CreateDefaultThreadLocal(factoryDelegate);
        }


        /// <summary>
        /// Creates the default thread local.
        /// </summary>
        /// <returns></returns>
        public IThreadLocal<T> CreateDefaultThreadLocal<T>(Func<T> factoryDelegate)
            where T : class
        {
            var localFactory = DefaultThreadLocalFactory;
            if (localFactory == null) {
                throw new ApplicationException("default thread local factory is not set");
            }

            return localFactory.CreateThreadLocal(factoryDelegate);
        }
    }
}
