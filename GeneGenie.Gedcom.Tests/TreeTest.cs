// <copyright file="TreeTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests
{
    using System.Text;
    using GeneGenui.Gedcom.Utility.Collections;
    using Utility.Enums;
    using Xunit;

    public abstract class TreeTest
    {
        protected Tree<string> fullTree;
        protected Tree<string> leftOnlyTree;
        protected Tree<string> rightOnlyTree;
        protected Tree<string> missingFinalRightTree;
        protected Tree<string> missingFinalLeftTree;

        protected string _FullTreeExpected;
        protected string _LeftOnlyTreeExpected;
        protected string _RightOnlyTreeExpected;
        protected string _MissingFinalRightTreeExpected;
        protected string _MissingFinalLeftTreeExpected;
        private TraversalType post;

        public TreeTest(TraversalType traversalType)
        {
            Tree<string> tree;
            TreeNode<string> root;
            TreeNode<string> node;

            fullTree = new Tree<string>();

            tree = fullTree;
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

            leftOnlyTree = new Tree<string>();

            tree = leftOnlyTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddLeft("B");
            node.AddLeft("C");

            rightOnlyTree = new Tree<string>();

            tree = rightOnlyTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddRight("B");
            node.AddRight("C");

            missingFinalRightTree = new Tree<string>();

            tree = missingFinalRightTree;
            tree.TraversalOrder = traversalType;
            root = new TreeNode<string>();
            root.Data = "A";
            tree.Root = root;

            node = root.AddLeft("B");
            node.AddLeft("C");

            node = root.AddRight("D");
            node.AddLeft("E");

            missingFinalLeftTree = new Tree<string>();

            tree = missingFinalLeftTree;
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
            var sb = new StringBuilder();
            foreach (TreeNode<string> treeNode in tree)
            {
                sb.Append(treeNode.Data);
            }

            string got = sb.ToString();
            Assert.Equal(expected, got);
        }
    }
}
