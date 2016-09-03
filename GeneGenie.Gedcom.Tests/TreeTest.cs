/*
 *  Copyright  (C) 2007 David A Knight <david@ritter.demon.co.uk>
 *  Amendments (C) 2016 Ryan O'Neill <r@genegenie.com>
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

namespace GeneGenie.Gedcom.Tests
{
    using System.Text;
    using Utility.Collections;
    using Xunit;

    public abstract class TreeTest
    {
        protected Tree<string> _FullTree;
        protected Tree<string> _LeftOnlyTree;
        protected Tree<string> _RightOnlyTree;
        protected Tree<string> _MissingFinalRightTree;
        protected Tree<string> _MissingFinalLeftTree;

        protected string _FullTreeExpected;
        protected string _LeftOnlyTreeExpected;
        protected string _RightOnlyTreeExpected;
        protected string _MissingFinalRightTreeExpected;
        protected string _MissingFinalLeftTreeExpected;

        public TreeTest(TraversalType traversalType)
        {
            Tree<string> tree;
            TreeNode<string> root;
            TreeNode<string> node;

            _FullTree = new Tree<string>();

            tree = _FullTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddLeft("B");
            node.AddLeft("C");
            node.AddRight("D");

            node = root.AddRight("E");
            node.AddLeft("F");
            node.AddRight("G");

            _LeftOnlyTree = new Tree<string>();

            tree = _LeftOnlyTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddLeft("B");
            node.AddLeft("C");

            _RightOnlyTree = new Tree<string>();

            tree = _RightOnlyTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddRight("B");
            node.AddRight("C");


            _MissingFinalRightTree = new Tree<string>();

            tree = _MissingFinalRightTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddLeft("B");
            node.AddLeft("C");

            node = root.AddRight("D");
            node.AddLeft("E");


            _MissingFinalLeftTree = new Tree<string>();

            tree = _MissingFinalLeftTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddLeft("B");
            node.AddRight("C");

            node = root.AddRight("D");
            node.AddRight("E");
        }

        public void DoTest(Tree<string> tree, string expected)
        {
            StringBuilder sb = new StringBuilder();
            foreach (TreeNode<string> treeNode in tree)
            {
                sb.Append(treeNode.Data);
            }

            string got = sb.ToString();
            Assert.Equal(got, expected);
        }
    }

}

