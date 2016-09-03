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
    using Utility.Collections;
    using Xunit;

    public class LevelOrderTreeTest : TreeTest
    {
        public LevelOrderTreeTest() : base(TraversalType.Level)
        {
            _FullTreeExpected = "ABECDFG";
            _LeftOnlyTreeExpected = "ABC";
            _RightOnlyTreeExpected = "ABC";
            _MissingFinalRightTreeExpected = "ABDCE";
            _MissingFinalLeftTreeExpected = "ABDCE";
        }

        [Fact]
        public void FullTree()
        {
            DoTest(_FullTree, _FullTreeExpected);
        }

        [Fact]
        public void LeftOnly()
        {
            DoTest(_LeftOnlyTree, _LeftOnlyTreeExpected);
        }

        [Fact]
        public void RightOnly()
        {
            DoTest(_RightOnlyTree, _RightOnlyTreeExpected);
        }

        [Fact]
        public void MissingFinalRight()
        {
            DoTest(_MissingFinalRightTree, _MissingFinalRightTreeExpected);
        }

        [Fact]
        public void MissingFinalLeft()
        {
            DoTest(_MissingFinalLeftTree, _MissingFinalLeftTreeExpected);
        }
    }

}
