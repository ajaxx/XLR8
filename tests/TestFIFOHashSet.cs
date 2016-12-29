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

using System.Linq;

using XLR8.Collections;

using NUnit.Framework;

namespace XLR8.Tests
{
    public class TestFIFOHashSet
    {
        [Test]
        public void TestAdd()
        {
            var collection = CreateDefaultCollection();

            Assert.That(collection.First(), Is.EqualTo(0));
            Assert.That(collection.Last(), Is.EqualTo(99));
            Assert.That(collection.Add(0), Is.True);
            Assert.That(collection.Add(100), Is.False);
            Assert.That(collection.Add(100), Is.True);
            Assert.That(collection.Remove(100), Is.True);
            Assert.That(collection.Add(100), Is.False);
        }

        [Test]
        public void TestRemove()
        {
            var collection = CreateDefaultCollection();

            Assert.That(collection.Count, Is.EqualTo(100));

            for (int ii = 99; ii >= 0; --ii)
            {
                Assert.That(collection.Remove(ii), Is.True);
                Assert.That(collection.Count, Is.EqualTo(ii));
                if (ii > 0)
                {
                    Assert.That(collection.First(), Is.EqualTo(0));
                    Assert.That(collection.Last(), Is.EqualTo(ii - 1));
                }
            }
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

        private static FIFOHashSet<int> CreateDefaultCollection()
        {
            var collection = new FIFOHashSet<int>();
            for (int ii = 0; ii < 100; ii++)
            {
                Assert.That(collection.Count, Is.EqualTo(ii));
                Assert.That(collection.Add(ii), Is.False);
                Assert.That(collection.Count, Is.EqualTo(ii + 1));
            }
            return collection;
        }
    }
}
