namespace GeneGenie.Gedcom.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Parser;

    public class AllGedcomLoading
    {
        [Benchmark(Baseline = true)]
        public void UseOriginalTryFinally()
        {
            var result = GedcomLoader.LoadAndParseOriginal("allged.ged");
        }

        [Benchmark]
        public void UsingStatementVersion()
        {
            var result = GedcomLoader.LoadAndParseUsing("allged.ged");
        }
    }
}
