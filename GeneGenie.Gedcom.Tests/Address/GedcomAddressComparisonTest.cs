// <copyright file="GedcomAddressComparisonTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Address.Tests
{
    using GeneGenie.Gedcom.Enums;
    using Xunit;

    /// <summary>
    /// Tests for equality of addresses.
    /// </summary>
    public class GedcomAddressComparisonTest
    {
        [Fact]
        private void Address_is_not_equal_to_null()
        {
            var address = new GedcomAddress();

            Assert.False(address.Equals(null));
        }

        [Fact]
        private void Address_is_not_equal_if_address_line_is_different()
        {
            var address1 = new GedcomAddress { AddressLine = "1234 Main St Anywhere" };
            var address2 = new GedcomAddress { AddressLine = "6789 Side St Somewhere" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_address_line1_is_different()
        {
            var address1 = new GedcomAddress { AddressLine1 = "1234 Main St" };
            var address2 = new GedcomAddress { AddressLine1 = "6789 Side St" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_address_line2_is_different()
        {
            var address1 = new GedcomAddress { AddressLine2 = "Anywhere" };
            var address2 = new GedcomAddress { AddressLine2 = "Somewhere" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_address_line3_is_different()
        {
            var address1 = new GedcomAddress { AddressLine3 = "PO Box 1234" };
            var address2 = new GedcomAddress { AddressLine3 = "Apt 123" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_equal_if_change_date_is_different()
        {
            var address1 = new GedcomAddress { ChangeDate = new GedcomChangeDate(null) { Date1 = "01 Jan 1900" } };
            var address2 = new GedcomAddress { ChangeDate = new GedcomChangeDate(null) { Date1 = "01 Jan 2000" } };

            Assert.True(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_city_is_different()
        {
            var address1 = new GedcomAddress { City = "City One" };
            var address2 = new GedcomAddress { City = "City Two" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_country_is_different()
        {
            var address1 = new GedcomAddress { Country = "USA" };
            var address2 = new GedcomAddress { Country = "Canada" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_email1_is_different()
        {
            var address1 = new GedcomAddress { Email1 = "email_one@domain" };
            var address2 = new GedcomAddress { Email1 = "email_two@another_domain" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_email2_is_different()
        {
            var address1 = new GedcomAddress { Email2 = "email_one@domain" };
            var address2 = new GedcomAddress { Email2 = "email_two@another_domain" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_email3_is_different()
        {
            var address1 = new GedcomAddress { Email3 = "email_one@domain" };
            var address2 = new GedcomAddress { Email3 = "email_two@another_domain" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_fax1_is_different()
        {
            var address1 = new GedcomAddress { Fax1 = "123-4567" };
            var address2 = new GedcomAddress { Fax1 = "999-9999" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_fax2_is_different()
        {
            var address1 = new GedcomAddress { Fax2 = "123-4567" };
            var address2 = new GedcomAddress { Fax2 = "999-9999" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_fax3_is_different()
        {
            var address1 = new GedcomAddress { Fax3 = "123-4567" };
            var address2 = new GedcomAddress { Fax3 = "999-9999" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_phone1_is_different()
        {
            var address1 = new GedcomAddress { Phone1 = "123-4567" };
            var address2 = new GedcomAddress { Phone1 = "999-9999" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_phone2_is_different()
        {
            var address1 = new GedcomAddress { Phone2 = "123-4567" };
            var address2 = new GedcomAddress { Phone2 = "999-9999" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_phone3_is_different()
        {
            var address1 = new GedcomAddress { Phone3 = "123-4567" };
            var address2 = new GedcomAddress { Phone3 = "999-9999" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_post_code_is_different()
        {
            var address1 = new GedcomAddress { PostCode = "12345" };
            var address2 = new GedcomAddress { PostCode = "67890" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_state_is_different()
        {
            var address1 = new GedcomAddress { State = "VA" };
            var address2 = new GedcomAddress { State = "CA" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_www1_is_different()
        {
            var address1 = new GedcomAddress { Www1 = "www.some-site.com" };
            var address2 = new GedcomAddress { Www1 = "www.another-site.edu" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_www2_is_different()
        {
            var address1 = new GedcomAddress { Www2 = "www.some-site.com" };
            var address2 = new GedcomAddress { Www2 = "www.another-site.edu" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Address_is_not_equal_if_www3_is_different()
        {
            var address1 = new GedcomAddress { Www3 = "www.some-site.com" };
            var address2 = new GedcomAddress { Www3 = "www.another-site.edu" };

            Assert.False(address1.CompareTo(address2) == 0);
        }

        [Fact]
        private void Addresses_are_equal_if_all_facts_are_equal()
        {
            var address1 = GenerateComparableAddress();
            var address2 = GenerateComparableAddress();

            Assert.True(address1.CompareTo(address2) == 0);
        }

        private GedcomAddress GenerateComparableAddress()
        {
            return new GedcomAddress
            {
                AddressLine = "1234 Main St Anywhere PO Box 1234",
                AddressLine1 = "1234 Main St",
                AddressLine2 = "Anywhere",
                AddressLine3 = "PO Box 1234",
                ChangeDate = new GedcomChangeDate(null) { DateType = GedcomDateType.Julian },
                City = "City One",
                Country = "USA",
                Email1 = "email1e@domain",
                Email2 = "email2@another_domain",
                Email3 = "email3@yet_another_domain",
                Fax1 = "333-3333",
                Fax2 = "666-6666",
                Fax3 = "999-9999",
                Phone1 = "111-1111",
                Phone2 = "222-2222",
                Phone3 = "333-3333",
                PostCode = "12345",
                State = "VA",
                Www1 = "www.some-site.com",
                Www2 = "www.some-other-site.edu",
                Www3 = "www.yet-another-site.net",
            };
        }
    }
}
