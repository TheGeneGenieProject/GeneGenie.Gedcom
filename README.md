# GeneGenie.Gedcom

## Current working dev branch for code quality, .net 7 and improved tests.

A .Net library for loading, saving, working with and analysing family trees stored in the GEDCOM format.

Thank you to David A Knight who developed Gedcom.Net from which this project was forked.

## Installation

Whilst we are below version 1.0 we won't be releasing a Nuget package is handled before then). To use the library, please download the source and star / set the repository as watched so you receive updates.

## Basic usage

If you would like to see a specific sample **please let us know what you want via Mastodon (@ryanoneill1970@mastodon.social or create an issue in GitHub**.

Check the sample project out for working code, basic operations are;

### Loading a tree

To load a tree into memory use the following static helper.

    var gedcomReader = GedcomRecordReader.CreateReader("Data\\presidents.ged");

There are other variants of this helper and non static methods that allow you to specify additional parameters such as encoding.

You'll want to make sure that the file you just read was parsed OK and handle any failures;

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

    var individual = new GedcomIndividualRecord(db);

    var name = individual.Names[0];
    name.Given = "Michael";
    name.Surname = "Mouse";
    name.Nick = "Mickey";

    individual.Names.Add(name);

    var birthDate = new GedcomDate(db);
    birthDate.ParseDateString("24 Jan 1933");
    individual.Events.Add(new GedcomIndividualEvent
    {
        Database = db,
        Date = birthDate,
        EventType = Enums.GedcomEventType.Birth
    });

### Saving the tree

    GedcomRecordWriter.OutputGedcom(db, "Rewritten.ged");

### Current build status
[![AppVeyor branch](https://img.shields.io/appveyor/ci/RyanONeill1970/genegenie-gedcom/master.svg)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-gedcom) [![NuGet](https://img.shields.io/nuget/v/GeneGenie.Gedcom.svg)](https://www.nuget.org/packages/GeneGenie.Gedcom) [![AppVeyor tests](https://img.shields.io/appveyor/tests/RyanONeill1970/genegenie-gedcom.svg)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-gedcom/build/tests)

### Code quality
[![Maintainability](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=GeneGenie.Gedcom) [![Quality gate](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=alert_status)](https://sonarcloud.io/dashboard?id=GeneGenie.Gedcom) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=bugs)](https://sonarcloud.io/component_measures?id=GeneGenie.Gedcom&metric=Reliability) [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=vulnerabilities)](https://sonarcloud.io/component_measures?id=GeneGenie.Gedcom&metric=Security) [![Code smells](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=code_smells)](https://sonarcloud.io/component_measures?id=GeneGenie.Gedcom&metric=Maintainability) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=coverage)](https://sonarcloud.io/component_measures?id=GeneGenie.Gedcom&metric=Coverage) [![Duplications](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=duplicated_lines_density)](https://sonarcloud.io/component_measures?id=GeneGenie.Gedcom&metric=Duplications) [![Reliability](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=GeneGenie.Gedcom) [![Security](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=security_rating)](https://sonarcloud.io/dashboard?id=GeneGenie.Gedcom) [![Security](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=sqale_index)](https://sonarcloud.io/dashboard?id=GeneGenie.Gedcom) [![Lines of code](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=ncloc)](https://sonarcloud.io/dashboard?id=GeneGenie.Gedcom)

[![Build stats](https://buildstats.info/appveyor/chart/ryanoneill1970/genegenie-gedcom)](https://ci.appveyor.com/project/ryanoneill1970/genegenie-gedcom/history)

## Contributing

We would love your help, see [Contributing.md](Contributing.md) for guidelines.
