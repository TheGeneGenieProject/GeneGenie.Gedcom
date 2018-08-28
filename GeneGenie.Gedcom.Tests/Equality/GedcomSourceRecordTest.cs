// <copyright file="GedcomSourceRecordTest.cs" company="GeneGenie.com">
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

namespace GeneGenie.Gedcom.Tests.Equality
{
    using System.Text;
    using Enums;
    using Xunit;

    /// <summary>
    /// Test suite for equality of GedcomSourceRecord
    /// </summary>
    public class GedcomSourceRecordTest
    {
        private readonly GedcomSourceRecord sourceRec1;
        private readonly GedcomSourceRecord sourceRec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomSourceRecordTest"/> class.
        /// </summary>
        public GedcomSourceRecordTest()
        {
            sourceRec1 = GenerateSourceRecord();
            sourceRec2 = GenerateSourceRecord();
        }

        [Fact]
        private void Source_record_is_not_equal_to_null()
        {
            Assert.NotNull(sourceRec1);
        }

        [Fact]
        private void Source_record_with_different_agency_is_not_equal()
        {
            sourceRec1.Agency = "agency one";
            sourceRec2.Agency = "agency two";

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_citation_is_not_equal()
        {
            sourceRec1.Citations.Add(new GedcomSourceCitation { Certainty = GedcomCertainty.Primary });
            sourceRec2.Citations.Add(new GedcomSourceCitation { Certainty = GedcomCertainty.Secondary });

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_data_note_is_not_equal()
        {
            sourceRec1.DataNotes.Add("data note one");
            sourceRec2.DataNotes.Add("data note two");

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_event_is_not_equal()
        {
            sourceRec1.EventsRecorded.Add(new GedcomRecordedEvent { Date = new GedcomDate { Date1 = "01/01/1980" } });
            sourceRec2.EventsRecorded.Add(new GedcomRecordedEvent { Date = new GedcomDate { Date1 = "12/12/2001" } });

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_filed_by_is_not_equal()
        {
            sourceRec1.FiledBy = "filed_by one";
            sourceRec2.FiledBy = "filed_by two";

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_originator_is_not_equal()
        {
            sourceRec1.Originator = "originator one";
            sourceRec2.Originator = "originator two";

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_originator_text_is_not_equal()
        {
            sourceRec1.OriginatorText = new StringBuilder("originator one");
            sourceRec2.OriginatorText = new StringBuilder("originator two");

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_publication_facts_is_not_equal()
        {
            sourceRec1.PublicationFacts = "fact one";
            sourceRec2.PublicationFacts = "fact two";

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_publication_text_is_not_equal()
        {
            sourceRec1.PublicationText = new StringBuilder("pub text one");
            sourceRec2.PublicationText = new StringBuilder("pub text two");

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_repository_citations_is_not_equal()
        {
            sourceRec1.RepositoryCitations.Add(new GedcomRepositoryCitation { Repository = "repo one" });
            sourceRec2.RepositoryCitations.Add(new GedcomRepositoryCitation { Repository = "repo two" });

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_text_is_not_equal()
        {
            sourceRec1.Text = "text one";
            sourceRec2.Text = "text two";

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_text_text_is_not_equal()
        {
            sourceRec1.TextText = new StringBuilder("text one");
            sourceRec2.TextText = new StringBuilder("text two");

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_title_is_not_equal()
        {
            sourceRec1.Title = "title one";
            sourceRec2.Title = "title two";

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_record_with_different_title_text_is_not_equal()
        {
            sourceRec1.TitleText = new StringBuilder("title one");
            sourceRec2.TitleText = new StringBuilder("title two");

            Assert.NotEqual(sourceRec1, sourceRec2);
        }

        [Fact]
        private void Source_records_with_same_facts_are_equal()
        {
            Assert.Equal(sourceRec1, sourceRec2);
        }

        private GedcomSourceRecord GenerateSourceRecord()
        {
            return new GedcomSourceRecord
            {
                Agency = "sample agency",
                Citations = { new GedcomSourceCitation() },
                DataNotes = { "sample data note" },
                EventsRecorded =
                {
                    new GedcomRecordedEvent { Date = new GedcomDate { Date1 = "03/03/1933" } },
                    new GedcomRecordedEvent { Date = new GedcomDate { Date1 = "04/04/1944" } }
                },
                FiledBy = "sample filed by",
                Originator = "sample originator",
                OriginatorText = new StringBuilder("sample originator text"),
                PublicationFacts = "sample publication facts",
                PublicationText = new StringBuilder("sample publication text"),
                RepositoryCitations =
                {
                    new GedcomRepositoryCitation { Repository = "repo one" },
                    new GedcomRepositoryCitation { Repository = "repo two" }
                },
                Text = "sample text",
                TextText = new StringBuilder("umm..."),
                Title = "sample title",
                TitleText = new StringBuilder("another sample title")
            };
        }
    }
}