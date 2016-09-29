# GeneGenie.Gedcom

A C# GEDCOM library for loading, saving, working with and analysing family trees stored in the GEDCOM format.

Thank you to David A Knight who developed Gedcom.Net from which this project was forked.

## Installation

Whilst we are below version 1.0 we won't be releasing a Nuget package (unless the .Net core / 4.5.x multi targeting issue [#5](../../issues/5) is handled before then). To use the library, please download the source and star / set the repository as watched so you receive updates.

## Basic usage

If you would like to see a specific sample **please let us know what you want via [Twitter (@ryanoneill1970)](https://twitter.com/ryanoneill1970) or create an issue in GitHub**.

Check the sample project out for working code, basic operations are;

### Loading a tree

To load a tree into memory use the following static helper.

    var gedcomReader = GedcomRecordReader.CreateReader("Data\\presidents.ged");

There are other variants of this helper and non static methods that allow you to specify additional parameters such as encoding.

You'll want to make sure that the file you just ready was parsed OK and handle any failures;

    if (gedcomReader.Parser.ErrorState != Parser.Enums.GedcomErrorState.NoError)
    {
        Console.WriteLine($"Could not read file, encountered error {gedcomReader.Parser.ErrorState}.");
    }

### Querying the tree

    Console.WriteLine($"Found {db.Families.Count} families and {db.Individuals.Count} individuals.");
    var individual = db
        .Individuals
        .FirstOrDefault(f => f.Names.Any());

    if (individual != null)
    {
        Console.WriteLine($"Individual found with a preferred name of '{individual.GetName()}'.");
    }

### Adding a person to the tree

    var individual = new GedcomIndividualRecord();

    var name = new GedcomName
    {
        Given = "Michael",
        Surname = "Mouse",
        Nick = "Mickey"
    };
    individual.Names.Add(name);

    var birthDate = new GedcomDate();
    birthDate.ParseDateString("24 Jan 1933");
    individual.Events.Add(new GedcomIndividualEvent
    {
        Date = birthDate,
        EventType = Enums.GedcomEventType.Birth
    });

### Saving the tree

## Status

In beta. Should be usable but we welcome bug reports and pull requests.

### Current appveyor status
[![Build status](https://ci.appveyor.com/api/projects/status/5o7cb79h0p7gci61?svg=true)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-gedcom)

## Contributing

We would love your help, subject to a few points. See [Contributing.md](Contributing.md) for guidelines.
