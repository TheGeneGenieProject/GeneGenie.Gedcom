/*
 *  $Id: Tree.cs 183 2008-06-08 15:31:15Z davek $
 * 
 *  Copyright (C) 2007 David A Knight <david@ritter.demon.co.uk>
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 *
 */

namespace Utility.Collections
{
    using System.Collections;

    public enum TraversalType
    {
        Pre,
        In,
        Post,
        Level
    };



    public class Tree<T> : IEnumerable
    {

        private TreeNode<T> _Root;

        private TraversalType _TraversalOrder;




        public Tree()
        {
        }





        public IEnumerator GetEnumerator()
        {
            return new TreeEnumerator<T>(this);
        }







        public TreeNode<T> Root
        {
            get { return _Root; }
            set { _Root = value; }
        }

        public TraversalType TraversalOrder
        {
            get { return _TraversalOrder; }
            set { _TraversalOrder = value; }
        }



    }

    public class TreeEnumerator<T> : IEnumerator
    {
        private Tree<T> _Tree;
        private TreeNode<T> _Current = null;

        public TreeEnumerator(Tree<T> tree)
        {
            _Tree = tree;
        }

        public object Current
        {
            get { return _Current; }
        }

        public bool MoveNext()
        {
            bool moved = false;

            switch (_Tree.TraversalOrder)
            {
                case TraversalType.Pre:
                    if (_Current == null)
                    {
                        _Current = _Tree.Root;
                        moved = (_Current != null);
                    }
                    else if (_Current.Left != null)
                    {
                        _Current = _Current.Left;
                        moved = true;
                    }
                    else if (_Current.Right != null)
                    {
                        _Current = _Current.Right;
                        moved = true;
                    }
                    else
                    {
                        while (_Current.Parent != null)
                        {
                            TreeNode<T> right = _Current;

                            _Current = _Current.Parent;
                            if (_Current != null && _Current.Right != right)
                            {
                                if (_Current.Right != null)
                                {
                                    _Current = _Current.Right;
                                    moved = true;
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case TraversalType.In:
                    if (_Current == null)
                    {
                        _Current = _Tree.Root;

                        while (_Current.Left != null)
                        {
                            _Current = _Current.Left;
                        }
                        moved = true;
                    }
                    else
                    {
                        if (_Current.Right == null)
                        {
                            TreeNode<T> current = _Current;
                            _Current = _Current.Parent;
                            while (_Current != null && _Current.Right == current)
                            {
                                current = _Current;
                                _Current = _Current.Parent;
                            }
                            moved = (_Current != null);
                        }
                        else
                        {
                            _Current = _Current.Right;
                            while (_Current.Left != null)
                            {
                                _Current = _Current.Left;
                            }
                            moved = true;
                        }
                    }

                    break;
                case TraversalType.Post:
                    if (_Current == null)
                    {
                        _Current = _Tree.Root;

                        while (true)
                        {
                            while (_Current.Left != null)
                            {
                                _Current = _Current.Left;
                            }
                            if (_Current.Right != null)
                            {
                                _Current = _Current.Right;
                            }
                            else
                            {
                                break;
                            }
                        }
                        moved = true;
                    }
                    else if (_Current.Parent != null)
                    {
                        TreeNode<T> current = _Current;

                        _Current = _Current.Parent;
                        if (_Current.Right != null && _Current.Right != current)
                        {
                            _Current = _Current.Right;
                            while (true)
                            {
                                while (_Current.Left != null)
                                {
                                    _Current = _Current.Left;
                                }
                                if (_Current.Right != null)
                                {
                                    _Current = _Current.Right;
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

        public void Reset()
        {
            _Current = null;
        }
    }
}
