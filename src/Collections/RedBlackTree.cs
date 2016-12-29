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

namespace XLR8.Collections
{
    public class RedBlackTree<TK,TV>
    {
        private readonly IComparer<TK> _comparer;
        private Node _root;
        private int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{TK, TV}"/> class.
        /// </summary>
        public RedBlackTree() : this(Comparer<TK>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedBlackTree{TK, TV}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public RedBlackTree(IComparer<TK> comparer)
        {
            _comparer = comparer;
            _count = 0;
            _root = null;
        }

        /// <summary>
        /// Clears the tree.
        /// </summary>
        public void Clear()
        {
            _root = null;
            _count = 0;
        }

        public void Add(TK key, TV value)
        {
            Insert(key, value);
        }

        public void Delete(TK key)
        {
            var node = Find(key);
            if (node == null)
                return;
            
            DeleteBST(node);
        }

        /// <summary>
        /// Deletes a node from the tree.
        /// </summary>
        /// <param name="node">The node.</param>
        private void DeleteBST(Node node)
        {
            if (node.Left == null && node.Right == null)
            {
                if (node.Parent == null)
                    _root = null;
                else if (node.IsLeft)
                    node.Parent.Left = null;
                else
                    node.Parent.Right = null;
            }
            else if (node.Left == null)
            {
                var vv = ReplaceParentWithChild(node.Right);
                var ss = vv.Sibling;
                var pp = vv.Parent;
                if (ss.IsRed)
                {
                    pp.InvertColor();
                    ss.InvertColor();
                }
            }
            else if (node.Right == null)
            {
                ReplaceParentWithChild(node.Left);
            }
            else
            {
                var leftMost = node.Leftmost;
                node.Key = leftMost.Key;
                node.Value = leftMost.Value;
                DeleteBST(leftMost);
            }



        }

        private Node ReplaceParentWithChild(Node node)
        {
            var parent = node.Parent;
            if (parent.Parent == null)
            {
                _root = node;
                node.Parent = null;
            }
            else if (parent.IsLeft)
            {
                parent.Parent.Left = node;
                node.Parent = node.Parent;
            }
            else
            {
                parent.Parent.Right = node;
                node.Parent = node.Parent;
            }

            return node;
        }

        /// <summary>
        /// Recursively descend through the tree to find the node with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>

        public Node Find(TK key)
        {
            return Find(_root, key);
        }

        /// <summary>
        /// Recursively descend through the tree to find the node with the given key.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public Node Find(Node root, TK key)
        {
            if (root == null)
                return null;

            var comp = _comparer.Compare(key, root.Key);
            if (comp < 0)
            {
                return Find(root.Left, key);
            }
            else if (comp > 0)
            {
                return Find(root.Right, key);
            }

            return root;
        }

        /// <summary>
        /// Inserts the value into the red-black tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private Node Insert(TK key, TV value)
        {
            if (_root == null)
            {
                _count = 1;
                _root = new Node(key, value, Color.BLACK);
                return _root;
            }

            var node = InsertBST(_root, key, value);
            // rebalance the tree
            _root = Rebalance(node);
            // return the node that was created
            return node;
        }

        /// <summary>
        /// Inserts the value into the red-black tree using a standard bst algorithm.
        /// This method returns the new node that has been added.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">key already exists</exception>
        private Node InsertBST(Node root, TK key, TV value)
        {
            if (root == null)
            {
                return new Node(key, value, Color.RED);
            }

            var comp = _comparer.Compare(key, root.Key);
            if (comp < 0)
            {
                root.Left = InsertBST(root.Left, key, value);
                root.Left.Parent = root;
                return root.Left;
            }
            else if (comp > 0)
            {
                root.Right = InsertBST(root.Right, key, value);
                root.Right.Parent = root;
                return root.Right;
            }
            else
            {
                throw new ArgumentException("key already exists");
            }
        }

        internal Node Rebalance(Node node)
        {
            while (node.Parent != null && node.Parent.Parent != null)
            {
                // get the parent
                var parent = node.Parent;

                // get the uncle
                var uncle = node.Sibling;
                var uncleIsRed = uncle != null && uncle.Color == Color.RED;

                var gparent = parent.Parent;

                if (parent.Color == Color.RED && uncleIsRed)
                {
                    parent.Color = Color.BLACK;
                    uncle.Color = Color.BLACK;
                    gparent.Color = Color.RED;
                    node = gparent;
                }
                else if (parent.Color == Color.RED && !uncleIsRed)
                {
                    if ((parent.IsLeft && node.IsRight) || (parent.IsRight && node.IsLeft))
                    {
                        // Zig-Zag
                        gparent.Color = Color.RED;
                        node.Color = Color.BLACK;
                        RotateLeft(parent);
                        RotateRight(gparent);
                    }
                    else
                    {
                        // Zig-Zig
                        parent.Color = Color.BLACK;
                        gparent.Color = Color.RED;
                        RotateRight(gparent);
                    }
                }
                else
                {
                    node = node.Parent;
                }
            }

            return node.Parent ?? node;
        }

        public Node RotateRight(Node root)
        {
            if (root == null)
                return null;

            var pp = root.Parent;

            var ll = root.Left;
            ll.Parent = pp;
            ll.Right = root;

            if (pp.Left == root)
                pp.Left = ll;
            else
                pp.Right = ll;

            root.Parent = ll;
            root.Left = ll.Left;

            if (root.Left != null)
                root.Left.Parent = root;

            return ll;
        }

        public Node RotateLeft(Node root)
        {
            if (root == null)
                return null;

            var pp = root.Parent;

            var rr = root.Right;
            rr.Parent = pp;
            rr.Left = root;

            if (pp.Left == root)
                pp.Left = rr;
            else
                pp.Right = rr;

            root.Parent = rr;
            root.Right = rr.Left;

            // reparent the right node (if exists)
            if (root.Right != null)
                root.Right.Parent = root;

            return rr;
        }

        public class Node
        {
            public Node Left;
            public Node Right;
            public Node Parent;
            public Color Color;
            public TK Key;
            public TV Value;

            public void InvertColor()
            {
                
            }

            public bool IsRed
            {
                get { return Color == Color.RED; }
            }

            public bool IsBlack
            {
                get { return Color == Color.BLACK; }
            }

            public bool IsLeft
            {
                get { return this == Parent.Left; }
            }

            public bool IsRight
            {
                get { return this == Parent.Right; }
            }

            public Node Root
            {
                get { return Parent != null ? Parent.Root : this; }
            }

            public Node Sibling
            {
                get
                {
                    if (Parent == null)
                        return null;
                    if (Parent.Left == this)
                        return Parent.Right;
                    return Parent.Left;
                }
            }

            public Node Grandparent
            {
                get { return Parent != null ? Parent.Parent : null; }
            }

            public Node Leftmost
            {
                get { return Left != null ? Left.Leftmost : this; }
            }

            public Node Rightmost
            {
                get { return Right != null ? Right.Rightmost : this; }
            }

            public Node(TK key, TV value, Color color)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
                Parent = null;
                Color = color;
            }

            public Node(TK key, TV value, Node left, Node right)
            {
                Key = key;
                Value = value;
                Left = left;
                Right = right;
            }

            public Node()
            {
            }
        }

        public enum Color
        {
            RED,
            BLACK
        };
    }
}
