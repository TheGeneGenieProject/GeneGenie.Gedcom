// <copyright file="Program.cs" company="GeneGenie.com">
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
    using BenchmarkDotNet.Running;

    /// <summary>
    /// Used for running benchmarks from the command line.
    /// CPU intensive.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main app entry point for benchmarks.
        /// </summary>
        /// <param name="args">Passed through to switcher to select the benchmark to run.</param>
        public static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[]
            {
                typeof(AllTagsBenchmark),
                typeof(GedFanBencharks)
            });
            switcher.Run(args);

            System.Console.WriteLine("Benchmarking complete, press a key to exit.");
            System.Console.ReadKey();
        }
    }
}