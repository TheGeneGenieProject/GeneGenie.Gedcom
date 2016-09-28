# Contributing

We would love your help, subject to a few points. If you submit code then you'll need to sign a Contributor Licence Agreement to state that your contribution is your own and we can use it, you'll also need to stick to the style guidelines.

**Don't worry if you are new to open source, just ask for help via [Twitter (@ryanoneill1970)](https://twitter.com/ryanoneill1970) or create an issue in GitHub.**

### Keep it simple

Try to keep your changes small and focussed on one thing so that when we merge, we can see what has been changed and review it easily. For example, changing typos in documentation and a minor bug fix should be two different commits.

If your changes need to be big, break them down into smaller steps. If you can't do that, talk to us first as it'll be easier to merge later on.

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
