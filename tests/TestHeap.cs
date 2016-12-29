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
using System.Linq;

using XLR8.Collections;

using NUnit.Framework;

namespace XLR8.Tests
{
    public class TestHeap
    {
        [Test]
        public void TestAdd()
        {
            var collection = CreateDefaultCollection(100);

            Assert.That(collection.Max, Is.EqualTo(99));

            collection = new Heap<int>();
            collection.Add(35, 33, 42, 10, 14, 19, 27, 44, 26, 31);

            var result = collection.ToString();
            Assert.That(result, Is.EqualTo("44{42{33{10,26},31{14}},35{19,27}}"));
        }

        [Test]
        public void TestHeapExpand()
        {
            var collection = CreateDefaultCollection(1000);
            Assert.That(collection.Count, Is.EqualTo(1000));
            Assert.That(collection.HeapLength, Is.EqualTo(1024));
        }

        [Test]
        public void TestRemove()
        {
            var collection = new Heap<int>();
            collection.Add(2, 13, 20, 17, 16, 6, 10);
            Console.WriteLine(collection.ToString());
        }

        [Test]
        public void TestClear()
        {
            var collection = CreateDefaultCollection();

            Assert.That(collection.Count, Is.EqualTo(100));
            collection.Clear();
            Assert.That(collection.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestEnumeration()
        {
            var collection = CreateDefaultCollection();
            var index = 0;

            foreach (var value in collection)
            {
                Assert.That(value, Is.EqualTo(index));
                index++;
            }
        }

        private static Heap<int> CreateDefaultCollection(int size = 100)
        {
            var collection = new Heap<int>();
            for (int ii = 0; ii < size; ii++)
            {
                Assert.That(collection.Count, Is.EqualTo(ii));
                collection.Add(ii);
                Assert.That(collection.Count, Is.EqualTo(ii + 1));
            }
            return collection;
        }
    }
}
