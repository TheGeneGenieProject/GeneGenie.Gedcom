namespace GeneGenie.Gedcom.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Parser;

    public class FourMbGedcomLoading
    {
        [Benchmark(Baseline = true)]
        public void UseOriginalTryFinally()
        {
            var result = GedcomLoader.LoadAndParseOriginal("gl120372.ged");
        }

        [Benchmark]
        public void UsingStatementVersion()
        {
            var result = GedcomLoader.LoadAndParseUsing("gl120372.ged");
        }
    }
}
