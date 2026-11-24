# GeneGenie.Gedcom

[![NuGet](https://img.shields.io/nuget/v/GeneGenie.Gedcom)](https://www.nuget.org/packages/GeneGenie.Gedcom)
[![NuGet downloads](https://img.shields.io/nuget/dt/GeneGenie.Gedcom)](https://www.nuget.org/packages/GeneGenie.Gedcom)
[![Build and run tests](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/actions/workflows/sonar.yml/badge.svg)](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/actions/workflows/sonar.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=coverage)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)

A .NET library for loading, saving, working with and analysing family trees stored in GEDCOM files. It
reads GEDCOM 5.5.1, along with parts of GEDCOM 7, and writes GEDCOM 5.5.1.

GeneGenie.Gedcom is part of [GeneGenie.com](https://genegenie.com), tools for researching your family
history.

## Installation

The library targets .NET 10. Add it to your project from [NuGet](https://www.nuget.org/packages/GeneGenie.Gedcom):

    dotnet add package GeneGenie.Gedcom

## Quick start

Read a file, check that it loaded, and write it back out:

    using GeneGenie.Gedcom.Enums;
    using GeneGenie.Gedcom.Parser;

    var reader = GedcomRecordReader.CreateReader("family.ged");

    if (reader.Parser.ErrorState != GedcomErrorState.NoError)
    {
        Console.WriteLine($"Could not read the file: {reader.Parser.ErrorState}.");
        return;
    }

    var db = reader.Database;
    Console.WriteLine($"Found {db.Individuals.Count} people and {db.Families.Count} families.");

    GedcomRecordWriter.OutputGedcom(db, "rewritten.ged");

## Learn more

- [Getting started](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/wiki/Getting-started): querying a tree and adding people to it.
- [Creating a GEDCOM file](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/wiki/Creating-a-GEDCOM-file): building a file in code, and saving part of an existing file.
- [Finding how two people are related](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/wiki/Finding-how-two-people-are-related): working out relationships such as "first cousin once removed".
- [The sample project](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/tree/main/GeneGenie.Gedcom.Sample): a small console app that loads, queries, changes and saves a tree.
- [Releases](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/releases): what changed in each version.

If there's a sample you'd like to see, please [open an issue](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/issues)
or ask on Mastodon at @ryanoneill1970@mastodon.social.

## Code quality

[![CodeQL](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/actions/workflows/codeql.yml/badge.svg)](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/actions/workflows/codeql.yml)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=bugs)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=GeneGenie.Gedcom&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)

[![Quality gate](https://sonarcloud.io/api/project_badges/quality_gate?project=GeneGenie.Gedcom)](https://sonarcloud.io/summary/new_code?id=GeneGenie.Gedcom)

## Contributing

We would love your help. See [Contributing.md](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/blob/dev/Contributing.md)
for how to get started, and the [coding conventions](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/blob/dev/docs/coding-conventions.md).

## Credits

Thank you to David A Knight, who wrote Gedcom.Net, from which this project was forked.

## Licence

GeneGenie.Gedcom is licensed under the [GNU Affero General Public License v3.0](https://github.com/TheGeneGenieProject/GeneGenie.Gedcom/blob/dev/Licence.txt).
