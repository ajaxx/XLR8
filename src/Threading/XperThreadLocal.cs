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
using System.Runtime.InteropServices;
using System.Threading;

namespace XLR8.Threading
{
    public sealed class XperThreadLocal<T> : IThreadLocal<T>
        where T : class
    {
        private readonly SlimLock _wLock;
        private readonly Func<T> _valueFactory;

        private int _primeIndex;

        /// <summary>
        /// NodeTable of nodes ...
        /// </summary>
        private Node[] _nodeTable;

        /// <summary>
        /// NodeTable that is indexed by hash code and points to the first node
        /// in the chain for that hash code.
        /// </summary>
        private int[] _hashIndex;

        private static readonly int[] PrimeTable =
            {
                67,
                131,
                257,
                521,
                1031,
                2053,
                4099,
                8209,
                16411,
                32771,
                65537,
                131101,
                262147,
                524309,
                1048583,
                2097169,
                4194319,
                8388617,
                16777259,
                33554467,
                67108879,
                134217757,
                268435459,
                536870923,
            };

        /// <summary>
        /// Indicates the index where the next node needs to be allocated from.
        /// </summary>
        private int _nodeAllocIndex = -1;

        /// <summary>
        /// Allocates a node for use and return the index of the node.
        /// </summary>
        /// <param name="threadId">The item.</param>
        /// <param name="value">The value.</param>
        /// <param name="hashCode">The hash code.</param>
        /// <returns></returns>
        private int AllocNode(int threadId, T value, int hashCode)
        {
            // Space must be allocated from the existing node table.
            int index = Interlocked.Increment(ref _nodeAllocIndex);
            if (index == _nodeTable.Length) {
                var newTableSize = _nodeTable.Length*2;
                var newTable = new Node[newTableSize];
                Array.Copy(_nodeTable, 0, newTable, 0, _nodeTable.Length);
                _nodeTable = newTable;
                ReIndex();
            }

            _nodeTable[index].SetValues(threadId, value, hashCode);
            return index;
        }

        /// <summary>
        /// Reindexes the internal bucket table.
        /// </summary>
        private void ReIndex()
        {
            int[] newHashIndex;
            bool hasCollision;

            do {
                // We assume there are no collisions going into the process
                hasCollision = false;
                // Create a new hash array of prime length
                int newHashIndexLength = PrimeTable[++_primeIndex];
                newHashIndex = new int[newHashIndexLength];
                // Reset the index values
                for (int ii = 0; ii < newHashIndexLength; ii++) {
                    newHashIndex[ii] = -1;
                }

                var nodeTable = _nodeTable;

                for (var nodeIndex = 0; nodeIndex < _nodeAllocIndex; nodeIndex++) {
                    var node = nodeTable[nodeIndex];
                    // Modulus the hash code with new table size 
                    var bucket = node.HashCode%newHashIndexLength;
                    if (newHashIndex[bucket] != -1) {
                        hasCollision = true;
                        break;
                    }
                    // Attach the node at the head of the bucket chain
                    newHashIndex[bucket] = nodeIndex;
                }

            } while (hasCollision);

            _hashIndex = newHashIndex;
        }

        private T SetValue(T value)
        {
            // Setting values causes the _hashIndex to become unstable ... we
            // should probably avoid chaining if at all possible too so that the
            // only element in the hash index is the element itself ...
            
            var nodeTable = _nodeTable;
            var hashIndex = _hashIndex;
            var thread = Thread.CurrentThread.ManagedThreadId;

            // Look for the node in the current space
            var hashCode = thread & 0x7fffffff;
            // Get the appropriate node index - remember there are no direct node references
            var chainIndex = hashIndex[hashCode % hashIndex.Length];

            // Skip entries that do not share the same hashcode
            if (chainIndex != -1) {
                if (nodeTable[chainIndex].HashCode == hashCode) {
                    if (nodeTable[chainIndex].ThreadId == thread) {
                        nodeTable[chainIndex].Value = value;
                        return value;
                    }
                }
            }

            _wLock.Enter();
            try {
                // Allocate a node
                var nodeIndex = AllocNode(thread, value, hashCode);
                // MapIndex the node
                while (true) {
                    hashIndex = _hashIndex;
                    chainIndex = hashCode%hashIndex.Length;
                    if (hashIndex[chainIndex] == -1) {
                        hashIndex[chainIndex] = nodeIndex;
                        return value;
                    }

                    ReIndex();
                }
            } finally {
                _wLock.Release();
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get
            {
                var nodeTable = _nodeTable;
                var hashIndex = _hashIndex;
                var thread = Thread.CurrentThread.ManagedThreadId;

                // Look for the node in the current space
                var hashCode = thread & 0x7fffffff;

                // Get the appropriate bucket - this indexes into HashIndex
                var hashIndexIndex = hashCode % hashIndex.Length;
                // Get the appropriate node index - remember there are no direct node references
                var headIndex = hashIndex[hashIndexIndex];

                if (headIndex != -1) {
                    // Skip entries that do not share the same hashcode
                    if (nodeTable[headIndex].HashCode == hashCode) {
                        // Check for node equality
                        if (thread == nodeTable[headIndex].ThreadId) {
                            return nodeTable[headIndex].Value;
                        }
                    }
                }

                return default(T);
            }

            set
            {
                SetValue(value);
            }
        }

        /// <summary>
        /// Gets the data or creates it if not found.
        /// </summary>
        /// <returns></returns>
        public T GetOrCreate() // ~32.26 ns/call
        {
            var nodeTable = _nodeTable;
            var hashIndex = _hashIndex;
            var thread = Thread.CurrentThread.ManagedThreadId;

            // Look for the node in the current space
            var hashCode = thread & 0x7fffffff;

            // Get the appropriate bucket - this indexes into HashIndex
            var hashIndexIndex = hashCode % hashIndex.Length;
            // Get the appropriate node index - remember there are no direct node references
            var headIndex = hashIndex[hashIndexIndex];
            if (headIndex != -1) {
                // Skip entries that do not share the same hashcode
                if (nodeTable[headIndex].HashCode == hashCode) {
                    // Check for node equality
                    if (nodeTable[headIndex].ThreadId == thread) {
                        return nodeTable[headIndex].Value;
                    }
                }
            }

            return SetValue(_valueFactory.Invoke());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XperThreadLocal{T}"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public XperThreadLocal(Func<T> factory)
        {
            _primeIndex = 0;

            var tableSize = PrimeTable[_primeIndex];

            _hashIndex = new int[tableSize];
            for (int ii = 0; ii < tableSize; ii++)
                _hashIndex[ii] = -1;

            _nodeTable = new Node[tableSize];

            _valueFactory = factory;
            _wLock = new SlimLock();
        }

        /// <summary>
        /// Each node contains the content for the node and references to
        /// the next node in it's respective chain and order.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        [Serializable]
        internal struct Node
        {
            /// <summary>
            /// Gets or sets the hash code.
            /// </summary>
            /// <value>The hash code.</value>
            internal int HashCode;

            /// <summary>
            /// Gets or sets the thread id.
            /// </summary>
            internal int ThreadId;

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            /// <value>The value.</value>
            internal T Value;

            internal void SetValues(int threadId, T value, int hashCode)
            {
                Value = value;
                ThreadId = threadId;
                HashCode = hashCode;
            }
        }
    }

    /// <summary>
    /// Creates slim thread local objects.
    /// </summary>
    public class XperThreadLocalFactory : ThreadLocalFactory
    {
        #region ThreadLocalFactory Members

        /// <summary>
        /// Create a thread local object of the specified type param.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory"></param>
        /// <returns></returns>
        public IThreadLocal<T> CreateThreadLocal<T>(Func<T> factory) where T : class
        {
            return new XperThreadLocal<T>(factory);
        }

        #endregion
    }
}
