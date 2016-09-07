// <copyright file="LevelOrderTreeTest.cs" company="GeneGenie.com">
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
    using Utility.Enums;
    using Xunit;

    public class LevelOrderTreeTest : TreeTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LevelOrderTreeTest"/> class.
        /// </summary>
        public LevelOrderTreeTest()
            : base(TraversalType.Level)
        {
            _FullTreeExpected = "ABECDFG";
            _LeftOnlyTreeExpected = "ABC";
            _RightOnlyTreeExpected = "ABC";
            _MissingFinalRightTreeExpected = "ABDCE";
            _MissingFinalLeftTreeExpected = "ABDCE";
        }

        [Fact]
        private void FullTree()
        {
            DoTest(fullTree, _FullTreeExpected);
        }

        [Fact]
        private void LeftOnly()
        {
            DoTest(leftOnlyTree, _LeftOnlyTreeExpected);
        }

        [Fact]
        private void RightOnly()
        {
            DoTest(rightOnlyTree, _RightOnlyTreeExpected);
        }

        [Fact]
        private void MissingFinalRight()
        {
            DoTest(missingFinalRightTree, _MissingFinalRightTreeExpected);
        }

        [Fact]
        private void MissingFinalLeft()
        {
            DoTest(missingFinalLeftTree, _MissingFinalLeftTreeExpected);
        }
    }
}
