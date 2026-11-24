# Coding conventions

These are the conventions for code and tests in GeneGenie.Gedcom. They apply to new code, and to code
you change substantially. Please don't reformat code you aren't otherwise changing: a small, focused
diff is easier to review (see [Contributing.md](../Contributing.md)).

The library is old and doesn't follow all of these everywhere yet. Where a rule conflicts with the
file you're editing, match the file and mention it in your pull request.

`.editorconfig` in the repository root is the first authority. Visual Studio, Rider and `dotnet
format` all read it, and where it differs from this document, it wins.

## Code

### File layout

Every `.cs` file starts with the licence header. `.editorconfig` marks a missing header as an error
(`IDE0073`), so your editor will flag it:

```csharp
// <copyright file="GedcomName.cs" company="GeneGenie.com">
// Copyright (c) GeneGenie.com. All Rights Reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE in the project root for license information.
// </copyright>
```

Namespaces are block-scoped (with braces), and `using` directives go inside the namespace. This is
what `.editorconfig` asks for, and it matches the rest of the codebase:

```csharp
namespace GeneGenie.Gedcom.Parser
{
    using System;
    using System.Text;

    public class GedcomRecordReader
    {
    }
}
```

Implicit usings aren't enabled, so add the `using` directives a file needs and remove any it no
longer uses.

### Document the public API

This is a library that other developers build on, so every public type and member needs an XML
documentation comment. Say why it exists and how to use it, not what its name already says. Many
older members have placeholder comments (`Gets or sets the title.`); if you touch one, please improve
it.

### Always use braces

Brace the body of every `if`, `else`, `for`, `foreach`, `while`, `do`, `using` and `lock`, even when it
is a single statement, and keep the body off the condition's line:

```csharp
if (value < 0)
{
    value = 0;
}
```

Not `if (value < 0) value = 0;`. A later edit that adds a second statement can't then fall outside the
condition by accident.

### Modern C# where it helps

The library targets .NET 9, so new code can use current language features:

- **Collection expressions** to initialise collections: `private readonly List<string> paths = [];`
  rather than `new List<string>()`.
- **Primary constructors** for new classes that take simple state.
- **Static classes** when a class has no instance state and all its members depend only on their
  arguments.

### Exceptions

- Catch the narrowest exception type you can actually handle. Don't catch `Exception` to make a failure
  disappear, and never leave a `catch` block empty.
- Rethrow with `throw;`, not `throw ex;`, so the original stack trace is kept.
- Check arguments with guard clauses such as `ArgumentNullException.ThrowIfNull(database)`.
- The reader is deliberately forgiving of bad GEDCOM files, because real files often break the
  specification. Prefer reporting a problem through the reader's existing errors and warnings over
  throwing, so one bad record doesn't stop a whole file loading.

### Name your numbers

A number whose meaning isn't obvious should be a named constant with a comment saying what it is and
where it comes from, for example a line-length limit from the GEDCOM specification. `0`, `1` and `-1`
are fine inline. Where .NET already names a value (`int.MaxValue`, `string.Empty`), use that rather than
declaring your own.

### Follow the GEDCOM specification

Use the specification's spelling for tags and values. Many programs write non-standard variants, and
the reader accepts some of them. When you add one, say in a comment which programs write it, so it's
clear why the code is there:

```csharp
case "UTF-8": // The GEDCOM 5.5.1 spelling.
case "UTF8": // Non-standard, but written by some programs.
```

### Keep the library's dependencies small

- Don't add a logging framework or other package dependency without discussing it in an issue first.
  Don't write to the console or debug output in new code.
- Where a value such as the current time can be passed in, pass it in rather than reading it inside
  the library, so the code can be tested.

### Don't break existing users

- Avoid changing the signatures or types of public members. If a change is needed, raise it in an
  issue first.
- Nullable reference types aren't enabled. Please don't add `#nullable enable` to existing files: it
  changes the annotations that users of the library see.

### No new warnings

The build still has some warnings, and the aim is to clear them and treat warnings as errors. Please
don't add new ones, and fix any in code you change.

## Tests

Tests use xUnit and live in `GeneGenie.Gedcom.Tests`, in the folder for the area they cover. Please
don't add a new test project. Run them all with `dotnet test` from the repository root; they must all
pass before a pull request is merged.

These rules apply to new tests. Existing tests don't have to be rewritten to match, but a new test
added to an existing file follows them.

### Start a bug fix with a failing test

Write a test that reproduces the problem as reported and fails for the reported reason. Then fix the
code, and run the whole suite.

### Arrange, act, assert

Every test has three blocks, separated by single blank lines: arrange, act, assert. Don't label them
with comments; the blank lines are the structure.

- **Act is one statement:** the call under test. Don't hide it inside the assertion.
- **Arrange holds only what this test needs.** Setup shared by every test in a class goes in the
  constructor; repeated setup goes in a well-named helper method.
- **Exceptions** are captured with `Record.Exception`, so act and assert stay separate.

### One assertion per test

Each test makes one assertion, so the name of a failing test tells you exactly what broke. To check
three things, write three tests.

- Use a `[Theory]` with `[InlineData]` to run one assertion against many inputs.
- `Assert.All` and `Assert.Single` count as one assertion.
- Keep assertions out of helper methods.

### Name tests as sentences

Test names are snake_case sentences describing the behaviour, with the first word capitalised:
`Surname_part_that_includes_the_prefix_is_kept_whole`, not `ReadName_SurnameCorrect`. Give each test
a `/// <summary>` saying why the behaviour matters.

### GEDCOM test data

Keep test input short and inline, so the test shows exactly what it reads. `TempGedcomFiles` in
`DataHelperExtensions/` writes the lines to a temporary file and deletes it after the test, and
`TempGedcomFiles.Gedcom(...)` adds the header and trailer:

```csharp
public sealed class GedcomSurnamePrefixTest : IDisposable
{
    private readonly TempGedcomFiles files = new();

    public void Dispose()
    {
        files.Dispose();
    }

    /// <summary>
    /// A file that splits the surname with SPFX and SURN keeps its prefix part.
    /// </summary>
    [Fact]
    public void Explicit_prefix_part_is_kept()
    {
        var path = files.Write(TempGedcomFiles.Gedcom("UTF-8", "0 @I1@ INDI", "1 NAME Hendrik /van Veen/", "2 SPFX van", "2 SURN Veen"));

        var reader = GedcomRecordReader.CreateReader(path);

        Assert.Equal("van", reader.Database.Individuals.Single().Names.Single().SurnamePrefix);
    }
}
```

Add a whole exported file to `Data/` only when a test needs one, and only with the agreement of the
person who sent it. Remove any details of living people first.

### Other test rules

- Don't sleep or wait on the clock in tests.
- Use the library's own constants rather than repeating their values, so a test can't keep checking an
  old limit after the limit changes.
