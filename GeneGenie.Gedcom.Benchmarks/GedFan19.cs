namespace GeneGenie.Gedcom.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Parser;

    public class GedFan19
    {
        [Benchmark(Baseline = true)]
        public void UseOriginalTryFinally()
        {
            var result = GedcomLoader.LoadAndParseOriginal("ENFAN19.ged");
        }

        [Benchmark]
        public void UsingStatementVersion()
        {
            var result = GedcomLoader.LoadAndParseUsing("ENFAN19.ged");
        }
    }
}
