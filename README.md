# GeneGenie.Gedcom

A C# GEDCOM library for loading, saving and analysing family trees stored in the GEDCOM format.

Thank you to David A Knight who developed Gedcom.Net from which this project was forked.

## Installation

Whilst we are below version 1.0 we won't be releasing a Nuget package. To use the library, please download the source and star / set the repository as watched so you receive updates.

## Usage

### Loading a tree
### Querying the tree
### Adding a person to the tree
### Saving the tree

## Status

In beta. Should be usable but we welcome bug reports and pull requests.

### Current appveyor status
[![Build status](https://ci.appveyor.com/api/projects/status/5o7cb79h0p7gci61?svg=true)](https://ci.appveyor.com/project/RyanONeill1970/genegenie-gedcom)

## Contributing

We'd love your help, subject to a few points. If you submit code then you'll need to sign a Contributor Licence Agreement to state that your contribution is your own and we can use it, plus you'll also need to stick to the style guidelines.

### Issues

If you find any problems and do not have the time to delve into the code to fix them, please open an issue with a concise test case containing the file or lines of GEDCOM data that caused the problem. Please also state the expected output.

### Unit tests and test data 

Any changes you commit should have passing unit tests covering them, see [the Microsoft style guidelines for unit tests](https://github.com/aspnet/Home/wiki/Engineering-guidelines#unit-tests-and-functional-tests). Unit tests should be written in xUnit.

If you can supply failing test data then it would be most appreciated.

### Documentation

Because this is a library used by other developers the methods, properties etc. that you create should be documented. There are lots of places where this documentation is lacking (TODO markers in comments or automatically generated nonsensical GhostDoc comments) which could really do with filling out. Don't comment code for the sake of it, comment the reason the code exists and how it might be used.

### Benchmarks

Major changes should have benchmarks wrapped around the old and new code to prove that we are not affecting performance. See the Benchmarks project for examples.

### Style guidelines

The StyleCop.Analyzer nuget package is used in all projects to help enforce style guidelines. These guidelines are there to ensure a consistent style, I don't necessarily agree with all of them but they represent a good compromise. When we remove the final few build warnings from the compiler, style violations will be treated as errors and will halt any build.
