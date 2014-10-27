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
using System.Collections.Generic;

namespace XLR8.Threading
{
    public class LockManager
    {
        /// <summary>
        /// Gets or sets the singleton instance.
        /// </summary>
        /// <value>
        /// The singleton instance.
        /// </value>
        public static LockManager Default { get; set; }

        /// <summary>
        /// Initializes the <see cref="LockManager"/> class.
        /// </summary>
        static LockManager()
        {
            Default = new LockManager();
        }

        private readonly Object _categoryFactoryTableLock = new object();
        private readonly IDictionary<string, Func<ILockable>> _categoryFactoryTable =
            new Dictionary<string, Func<ILockable>>();

        /// <summary>
        /// Gets or sets the default lock factory.
        /// </summary>
        /// <value>The default lock factory.</value>
        public Func<ILockable> DefaultLockFactory { get; set; }

        /// <summary>
        /// Initializes the <see cref="LockManager"/> class.
        /// </summary>
        public LockManager(Func<ILockable> lockFactory = null)
        {
            if (lockFactory == null)
            {
                lockFactory = MonitorLock;
            }

            DefaultLockFactory = lockFactory;
        }

        /// <summary>
        /// Registers the category lock.
        /// </summary>
        /// <param name="typeCategory">The type category.</param>
        /// <param name="lockFactory">The lock factory.</param>
        public void RegisterCategoryLock(Type typeCategory, Func<ILockable> lockFactory)
        {
            RegisterCategoryLock(typeCategory.FullName, lockFactory);
        }

        /// <summary>
        /// Registers the category lock.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="lockFactory">The lock factory.</param>
        public void RegisterCategoryLock(string category, Func<ILockable> lockFactory)
        {
            lock( _categoryFactoryTableLock ) {
                _categoryFactoryTable[category] = lockFactory;
            }
        }

        /// <summary>
        /// Creates a lock for the category defined by the type.
        /// </summary>
        /// <param name="typeCategory">The type category.</param>
        /// <returns></returns>
        public ILockable CreateLock(Type typeCategory)
        {
            return CreateLock(typeCategory.FullName);
        }

        /// <summary>
        /// Creates a lock for the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public ILockable CreateLock(string category)
        {
            if (category != null) {
                lock (_categoryFactoryTableLock) {
                    category = category.TrimEnd('.');

                    while( category != String.Empty ) {
                        Func<ILockable> lockFactory;
                        // Lookup a factory for the category
                        if (_categoryFactoryTable.TryGetValue(category, out lockFactory)) {
                            return lockFactory.Invoke();
                        }
                        // Lock factory not found, back-up one segment of the category
                        int lastIndex = category.LastIndexOf('.');
                        if (lastIndex == -1) {
                            break;
                        }

                        category = category.Substring(0, lastIndex).TrimEnd('.');
                    }
                }
            }

            return CreateDefaultLock();
        }

        /// <summary>
        /// Creates the default lock.
        /// </summary>
        /// <returns></returns>
        public ILockable CreateDefaultLock()
        {
            var lockFactory = DefaultLockFactory;
            if (lockFactory == null) {
                throw new ApplicationException("default lock factory is not set");
            }

            return lockFactory.Invoke();
        }

        /// <summary>
        /// Creates the monitor lock.
        /// </summary>
        /// <returns></returns>
        public static ILockable MonitorLock()
        {
            return new MonitorLock();
        }

        /// <summary>
        /// Creates the monitor spin lock.
        /// </summary>
        /// <returns></returns>
        public static ILockable MonitorSpinLock()
        {
            return new MonitorSpinLock();
        }

        /// <summary>
        /// Creates the monitor slim lock.
        /// </summary>
        /// <returns></returns>
        public static ILockable MonitorSlimLock()
        {
            return new MonitorSlimLock();
        }
    }
}
