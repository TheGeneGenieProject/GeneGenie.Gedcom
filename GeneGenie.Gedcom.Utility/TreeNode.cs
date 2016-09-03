/*
 *  $Id: TreeNode.cs 183 2008-06-08 15:31:15Z davek $
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
    using System;

    public class TreeNode<T>
    {

        private TreeNode<T> _Parent;
        private TreeNode<T> _Left;
        private TreeNode<T> _Right;

        private T _Data;




        public TreeNode()
        {
        }




        public TreeNode<T> Parent
        {
            get { return _Parent; }
            set { _Parent = value; }
        }

        public TreeNode<T> Left
        {
            get { return _Left; }
            set { _Left = value; }
        }

        public TreeNode<T> Right
        {
            get { return _Right; }
            set { _Right = value; }
        }

        public T Data
        {
            get { return _Data; }
            set { _Data = value; }
        }




        public void AddLeft(TreeNode<T> node)
        {
            if (node.Parent != null)
            {
                throw new Exception("node already has a parent");
            }
            if (Left != null)
            {
                throw new Exception("Left node already present");
            }

            this.Left = node;
            node.Parent = this;
        }

        public TreeNode<T> AddLeft(T data)
        {
            TreeNode<T> node = new TreeNode<T>();
            node.Data = data;
            AddLeft(node);

            return node;
        }

        public void AddRight(TreeNode<T> node)
        {
            if (node.Parent != null)
            {
                throw new Exception("node already has a parent");
            }
            if (Right != null)
            {
                throw new Exception("Right node already present");
            }

            this.Right = node;
            node.Parent = this;
        }

        public TreeNode<T> AddRight(T data)
        {
            TreeNode<T> node = new TreeNode<T>();
            node.Data = data;
            AddRight(node);

            return node;
        }

        public void Detach()
        {
            if (Parent == null)
            {
                throw new Exception("node already detached");
            }

            if (this == Parent.Left)
            {
                Parent.Left = null;
            }
            else
            {
                Parent.Right = null;
            }
            Parent = null;
        }


    }
}
