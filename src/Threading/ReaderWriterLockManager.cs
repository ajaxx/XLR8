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
    public class ReaderWriterLockManager
    {
        /// <summary>
        /// Gets or sets the singleton instance.
        /// </summary>
        /// <value>
        /// The singleton instance.
        /// </value>
        public static ReaderWriterLockManager SingletonInstance { get; set; }

        /// <summary>
        /// Initializes the <see cref="ReaderWriterLockManager"/> class.
        /// </summary>
        static ReaderWriterLockManager()
        {
            SingletonInstance = new ReaderWriterLockManager();
        }
        
        private readonly Object _categoryFactoryTableLock = new object();

        private readonly IDictionary<string, Func<IReaderWriterLock>> _categoryFactoryTable =
            new Dictionary<string, Func<IReaderWriterLock>>();

        /// <summary>
        /// Gets or sets the default lock factory.
        /// </summary>
        /// <value>The default lock factory.</value>
        public Func<IReaderWriterLock> DefaultLockFactory { get; set; }

        /// <summary>
        /// Initializes the <see cref="LockManager"/> class.
        /// </summary>
        public ReaderWriterLockManager(Func<IReaderWriterLock> lockFactory = null)
        {
            if (lockFactory == null)
                lockFactory = SlimLock;

            DefaultLockFactory = lockFactory;
        }
        
        /// <summary>
        /// Registers the category lock.
        /// </summary>
        /// <param name="lockFactory">The lock factory.</param>
        public void RegisterCategoryLock<T>(Func<IReaderWriterLock> lockFactory)
        {
        	RegisterCategoryLock(typeof(T).FullName, lockFactory);
        }

        /// <summary>
        /// Registers the category lock.
        /// </summary>
        /// <param name="typeCategory">The type category.</param>
        /// <param name="lockFactory">The lock factory.</param>
        public void RegisterCategoryLock(Type typeCategory, Func<IReaderWriterLock> lockFactory)
        {
            RegisterCategoryLock(typeCategory.FullName, lockFactory);
        }

        /// <summary>
        /// Registers the category lock.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <param name="lockFactory">The lock factory.</param>
        public void RegisterCategoryLock(string category, Func<IReaderWriterLock> lockFactory)
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
        public IReaderWriterLock CreateLock(Type typeCategory)
        {
            var typeName = typeCategory.FullName;
            if (typeName != null) {
                var typeNameIndex = typeName.IndexOf('`');
                if (typeNameIndex != -1) {
                    typeName = typeName.Substring(0, typeNameIndex);
                }
            }

            return CreateLock(typeName);
        }

        /// <summary>
        /// Creates a lock for the category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        public IReaderWriterLock CreateLock(string category)
        {
            var trueCategory = category;

            if (category != null) {
                lock (_categoryFactoryTableLock) {
                    trueCategory = category = category.TrimEnd('.');

                    while( category != String.Empty ) {
                        Func<IReaderWriterLock> lockFactory;
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

            return CreateDefaultLock(trueCategory);
        }

        /// <summary>
        /// Creates the default lock.
        /// </summary>
        /// <returns></returns>
        public IReaderWriterLock CreateDefaultLock()
        {
            return CreateDefaultLock(string.Empty);
        }

        /// <summary>
        /// Creates the default lock.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns></returns>
        private IReaderWriterLock CreateDefaultLock(string category)
        {
            var lockFactory = DefaultLockFactory;
            if (lockFactory == null) {
                throw new ApplicationException("default lock factory is not set");
            }

            return lockFactory.Invoke();
        }

        /// <summary>
        /// Creates a singularity lock.
        /// </summary>
        /// <returns></returns>
        public static IReaderWriterLock SingularityLock()
        {
            return new DummyReaderWriterLock();
        }

        /// <summary>
        /// Creates the standard reader writer lock.
        /// </summary>
        /// <returns></returns>
        public static IReaderWriterLock StandardLock()
        {
            return new StandardReaderWriterLock();
        }

        /// <summary>
        /// Creates the slim reader writer lock.
        /// </summary>
        /// <returns></returns>
        public static IReaderWriterLock SlimLock()
        {
            return new SlimReaderWriterLock();
        }

        /// <summary>
        /// Creates the void reader writer lock.
        /// </summary>
        /// <returns></returns>
        public static IReaderWriterLock VoidLock()
        {
            return new VoidReaderWriterLock();
        }

        /// <summary>
        /// Creates the fair reader writer lock.
        /// </summary>
        /// <returns></returns>
        public static IReaderWriterLock FairLock()
        {
            return new FairReaderWriterLock();
        }
        
        /// <summary>
        /// Creates the fifo reader writer lock.
        /// </summary>
        /// <returns></returns>
        public static IReaderWriterLock FifoLock()
        {
            return new FifoReaderWriterLock();
        }
    }
}
