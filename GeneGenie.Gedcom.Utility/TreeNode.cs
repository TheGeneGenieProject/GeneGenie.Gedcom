// <copyright file="TreeNode.cs" company="GeneGenie.com">
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
    using System;

    public class TreeNode<T>
    {
        public TreeNode<T> Parent { get; set; }

        public TreeNode<T> Left { get; set; }

        public TreeNode<T> Right { get; set; }

        public T Data { get; set; }

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
