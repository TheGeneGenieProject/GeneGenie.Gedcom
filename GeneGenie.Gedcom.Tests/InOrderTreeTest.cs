// <copyright file="InOrderTreeTest.cs" company="GeneGenie.com">
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

    public class InOrderTreeTest : TreeTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InOrderTreeTest"/> class.
        /// </summary>
        public InOrderTreeTest()
            : base(TraversalType.In)
        {
            _FullTreeExpected = "CBDAFEG";
            _LeftOnlyTreeExpected = "CBA";
            _RightOnlyTreeExpected = "ABC";
            _MissingFinalRightTreeExpected = "CBAED";
            _MissingFinalLeftTreeExpected = "BCADE";
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
