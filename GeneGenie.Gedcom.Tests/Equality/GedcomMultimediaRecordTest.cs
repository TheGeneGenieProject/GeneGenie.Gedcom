// <copyright file="GedcomMultimediaRecordTest.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>

namespace GeneGenie.Gedcom.Tests.Equality
{
    using Xunit;

    /// <summary>
    /// Tests for equality of multimedia records.
    /// </summary>
    public class GedcomMultimediaRecordTest
    {
        private GedcomMultimediaRecord multRec1;
        private GedcomMultimediaRecord multRec2;

        /// <summary>
        /// Initializes a new instance of the <see cref="GedcomMultimediaRecordTest"/> class.
        /// </summary>
        public GedcomMultimediaRecordTest()
        {
            multRec1 = GenerateMultimediaRecord();
            multRec2 = GenerateMultimediaRecord();
        }

        [Fact]
        private void Multimedia_record_is_not_equal_to_null()
        {
            Assert.NotNull(multRec1);
        }

        [Fact]
        private void Multimedia_record_with_different_files_is_not_equal()
        {
            multRec1.Files.Add(new GedcomMultimediaFile { Filename = "some_file" });
            multRec2.Files.Add(new GedcomMultimediaFile { Filename = "another_file" });

            Assert.NotEqual(multRec1, multRec2);
        }

        [Fact]
        private void Multimedia_record_with_different_title_is_not_equal()
        {
            multRec1.Title = "first title";
            multRec2.Title = "second title";

            Assert.NotEqual(multRec1, multRec2);
        }

        [Fact]
        private void Multimedia_records_with_same_facts_are_equal()
        {
            Assert.Equal(multRec1, multRec2);
        }

        private GedcomMultimediaRecord GenerateMultimediaRecord()
        {
            return new GedcomMultimediaRecord
            {
                Title = "multimedia title",
                Files =
                {
                    new GedcomMultimediaFile { Filename = "file_name_one" },
                    new GedcomMultimediaFile { Filename = "file_name_two" },
                },
            };
        }
    }
}
