namespace GeneGenie.Gedcom.Sample
{
    using Parser;
    using System;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            var db = LoadGedcom();
            if (db == null)
                return;

            OutputGedcomStats(db);

            Console.WriteLine("Press a key to continue.");
            Console.ReadKey();
        }

        private static GedcomDatabase LoadGedcom()
        {
            var gedcomReader = new GedcomRecordReader();
            if (gedcomReader.ReadGedcom("Data\\presidents.ged") == false)
            {
                Console.WriteLine("Could not read file, press a key to continue.");
                Console.ReadKey();
                return null;
            }
            return gedcomReader.Database;
        }

        private static void OutputGedcomStats(GedcomDatabase db)
        {
            Console.WriteLine($"Found {db.Families.Count} families and {db.Individuals.Count} individuals.");
            var individual = db
                .Individuals
                .FirstOrDefault(f => f.Notes.Any());

            if (individual == null)
            {
                Console.WriteLine($"Notes missing from GEDCOM file.");
            }
            else
            {
                var name = individual.GetName();
                Console.WriteLine($"Notes found for individual named '{name.Name}'.");
                foreach (var note in individual.Notes)
                {
                    Console.WriteLine(note);
                }
            }
        }
    }
}
