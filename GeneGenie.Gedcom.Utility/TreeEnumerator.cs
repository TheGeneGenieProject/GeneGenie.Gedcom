// <copyright file="TreeEnumerator.cs" company="GeneGenie.com">
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
//
// You should have received a copy of the GNU Affero General Public License
// along with this program. If not, see http:www.gnu.org/licenses/ .
//
// </copyright>
// <author> Copyright (C) 2007 David A Knight david@ritter.demon.co.uk </author>
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenui.Gedcom.Utility.Collections
{
    using System.Collections;
    using GeneGenie.Gedcom.Utility.Enums;

    public class TreeEnumerator<T> : IEnumerator
    {
        private Tree<T> tree;
        private TreeNode<T> current = null;

        public TreeEnumerator(Tree<T> tree)
        {
            this.tree = tree;
        }

        public object Current
        {
            get { return current; }
        }

        public bool MoveNext()
        {
            bool moved = false;

            switch (tree.TraversalOrder)
            {
                case TraversalType.Pre:
                    if (current == null)
                    {
                        current = tree.Root;
                        moved = current != null;
                    }
                    else if (current.Left != null)
                    {
                        current = current.Left;
                        moved = true;
                    }
                    else if (current.Right != null)
                    {
                        current = current.Right;
                        moved = true;
                    }
                    else
                    {
                        while (current.Parent != null)
                        {
                            TreeNode<T> right = current;

                            current = current.Parent;
                            if (current != null && current.Right != right)
                            {
                                if (current.Right != null)
                                {
                                    current = current.Right;
                                    moved = true;
                                    break;
                                }
                            }
                        }
                    }

                    break;
                case TraversalType.In:
                    if (current == null)
                    {
                        current = tree.Root;

                        while (current.Left != null)
                        {
                            current = current.Left;
                        }

                        moved = true;
                    }
                    else
                    {
                        if (current.Right == null)
                        {
                            TreeNode<T> currentNode = current;
                            current = current.Parent;
                            while (current != null && current.Right == currentNode)
                            {
                                currentNode = current;
                                current = current.Parent;
                            }

                            moved = current != null;
                        }
                        else
                        {
                            current = current.Right;
                            while (current.Left != null)
                            {
                                current = current.Left;
                            }

                            moved = true;
                        }
                    }

                    break;
                case TraversalType.Post:
                    if (current == null)
                    {
                        current = tree.Root;

                        while (true)
                        {
                            while (current.Left != null)
                            {
                                current = current.Left;
                            }

                            if (current.Right != null)
                            {
                                current = current.Right;
                            }
                            else
                            {
                                break;
                            }
                        }

                        moved = true;
                    }
                    else if (current.Parent != null)
                    {
                        TreeNode<T> currentNode = current;

                        current = current.Parent;
                        if (current.Right != null && current.Right != currentNode)
                        {
                            current = current.Right;
                            while (true)
                            {
                                while (current.Left != null)
                                {
                                    current = current.Left;
                                }

                                if (current.Right != null)
                                {
                                    current = current.Right;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        moved = true;
                    }

                    break;
                case TraversalType.Level:

                    break;
            }

            return moved;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            current = null;
        }
    }
}
