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

using System.Collections.Generic;

namespace XLR8.Collections
{
    public class Treap<TK, TV>
    {
        private Node _root;
        private IComparer<TK> _comparer;

        internal Node Set(TK key, TV val, Node root, int priority)
        {
            if (root == null)
                return new Node(priority, key, val, null, null);

            var cmp = _comparer.Compare(key, root.Key);
            if (cmp == 0)
            {
                return new Node(root.Priority, key, val, root.Left, root.Right);
            }
            else if (cmp < 0)
            {
                var node = new Node(
                    root.Priority,
                    root.Key,
                    root.Val,
                    Set(key, val, root.Left, priority),
                    root.Right);
                return (node.Left.Priority < node.Priority) ? RotateLeft(node) : node;
            }
            else
            {
                var node = new Node(
                    root.Priority,
                    root.Key,
                    root.Val,
                    root.Left,
                    Set(key, val, root.Right, priority));
                return (node.Right.Priority < node.Priority) ? RotateRight(node) : node;
            }
        }

        internal bool Contains(TK key)
        {
            return Find(key, _root) != null;
        }

        internal Node Find(TK key, Node node)
        {
            if (node == null)
                return null;

            var cmp = _comparer.Compare(key, node.Key);
            if (cmp == 0)
                return node;
            else if (cmp < 0)
                return Find(key, node.Left);
            else
                return Find(key, node.Right);
        }

        private Node RightMost(Node reference)
        {
            var node = reference;
            while (node.Right != null)
                node = node.Right;
            return node;
        }

        private Node LeftMost(Node reference)
        {
            var node = reference;
            while (node.Left != null)
                node = node.Left;
            return node;
        }

        private Node RotateRight(Node node)
        {
            return new Node(
                node.Right.Priority,
                node.Right.Key,
                node.Right.Val,
                new Node(
                    node.Priority,
                    node.Key,
                    node.Val,
                    node.Left,
                    node.Right.Left
                ),
                node.Right.Right
            );
        }

        private Node RotateLeft(Node node)
        {
            return new Node(
                node.Left.Priority,
                node.Left.Key,
                node.Left.Val,
                node.Left.Left,
                new Node(
                    node.Priority,
                    node.Key,
                    node.Val,
                    node.Left.Right,
                    node.Right
                )
            );
        }

        public class Node
        {
            internal int Priority;
            internal TK Key;
            internal TV Val;
            internal Node Left;
            internal Node Right;

            public Node(int priority, TK key, TV val, Node left, Node right)
            {
                Priority = priority;
                Key = key;
                Val = val;
                Left = left;
                Right = right;
            }
        }
    }
}
