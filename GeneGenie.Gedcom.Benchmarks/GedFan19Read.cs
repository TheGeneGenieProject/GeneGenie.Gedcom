namespace GeneGenie.Gedcom.Benchmarks
{
    using BenchmarkDotNet.Attributes;
    using Parser;

    public class GedFan19Read
    {
        public void LoadTimeBenchmark()
        {
            var _reader = new GedcomRecordReader();
            bool success = _reader.ReadGedcom("Data\\ENFAN19");
        }
    }
}
