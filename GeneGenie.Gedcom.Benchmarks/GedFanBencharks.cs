// <copyright file="GedFanBencharks.cs" company="GeneGenie.com">
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
    /// Set of benchmarks to show how we lay out benchmarking code.
    /// These are not direct comparisons as they double in size at every
    /// generation but are good for demonstration purposes.
    /// Benchmark files used were generated with GedFan
    /// (http://www.tamurajones.net/GedFan.xhtml) by Tamura Jones.
    /// </summary>
    public class GedFanBencharks
    {
        /// <summary>Benchmark for loading and parsing a 1 generation GEDCOM file.</summary>
        [Benchmark(Baseline = true)]
        public void One_generation()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN1.ged");
        }

        /// <summary>Benchmark for loading and parsing a 2 generation GEDCOM file.</summary>
        [Benchmark]
        public void Two_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN2.ged");
        }

        /// <summary>Benchmark for loading and parsing a 3 generation GEDCOM file.</summary>
        [Benchmark]
        public void Three_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN3.ged");
        }

        /// <summary>Benchmark for loading and parsing a 4 generation GEDCOM file.</summary>
        [Benchmark]
        public void Four_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN4.ged");
        }

        /// <summary>Benchmark for loading and parsing a 5 generation GEDCOM file.</summary>
        [Benchmark]
        public void Five_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN5.ged");
        }

        /// <summary>Benchmark for loading and parsing a 6 generation GEDCOM file.</summary>
        [Benchmark]
        public void Six_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN6.ged");
        }

        /// <summary>Benchmark for loading and parsing a 7 generation GEDCOM file.</summary>
        [Benchmark]
        public void Seven_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN7.ged");
        }

        /// <summary>Benchmark for loading and parsing a 8 generation GEDCOM file.</summary>
        [Benchmark]
        public void Eight_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN8.ged");
        }

        /// <summary>Benchmark for loading and parsing a 9 generation GEDCOM file.</summary>
        [Benchmark]
        public void Nine_generations()
        {
            var loader = new GedcomLoader();
            loader.LoadAndParse("ENFAN9.ged");
        }
    }
}
