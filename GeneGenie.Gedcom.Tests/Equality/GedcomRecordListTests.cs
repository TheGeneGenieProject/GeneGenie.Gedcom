// <copyright file="GedcomRecordListTests.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
// <author> Copyright (C) 2018 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Tests.Equality
{
    using Xunit;

    public class GedcomRecordListTests
    {
        [Fact]
        private void Hash_codes_for_identical_lists_are_the_same()
        {
            var list1 = new GedcomRecordList<string> { "item 1" };
            var list2 = new GedcomRecordList<string> { "item 1" };

            Assert.Equal(list1.GetHashCode(), list2.GetHashCode());
        }
    }
}
