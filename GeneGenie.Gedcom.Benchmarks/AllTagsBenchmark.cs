// <copyright file="AllTagsBenchmark.cs" company="GeneGenie.com">
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
// <author> Copyright (C) 2016 Ryan O'Neill r@genegenie.com </author>

namespace GeneGenie.Gedcom.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Parser;

    /// <summary>
    /// Uses the standard file provided by H. Eichmann to benchmark loading all known GEDCOM tags.
    /// </summary>
    public class AllTagsBenchmark
    {
        /// <summary>
        /// Benchmarks the loading of all known gedcom tags.
        /// </summary>
        [Benchmark(Baseline = true)]
        public void Load_all_known_gedcom_tags()
        {
            GedcomLoader.LoadAndParse("allged.ged");
        }
    }
}
