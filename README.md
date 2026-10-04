# stampeded-demo

A small C# solution with a handful of staged pull requests, kept open on purpose. It is the
repository the [Stampeded!](https://github.com/icsharpcode/Stampeded) feature tours are
written against: every screenshot in them was taken here, and every step can be repeated with
Repository > Open from URL and `christophwille/stampeded-demo`.

The code is a toy - a herd of cattle, who owns which brand, and what the herd is worth:

- `src/Corral` - the library
- `src/Corral.Cli` - prints a sample herd (`dotnet run --project src/Corral.Cli`)
- `tests/Corral.Tests` - NUnit

## The pull requests are staged

| Pull request | What it is there to show |
| --- | --- |
| Price herds by weight class | a change in several commits, with a rename, a removed method and review threads |
| Extract brand registry | a branch that gets force-pushed between two readings |
| Cache herd totals | a failing test and code the tests do not reach |
| Fix typo in CLI help | something to merge |

Please do not merge or close them. `stage.ps1` puts everything back the way the tours expect
(it force-pushes, so it needs write access - fork the repository to run it yourself):

    ./stage.ps1          # branches, pull requests and seed comments, as first read
    ./stage.ps1 -Push2   # the force push the re-review tour is about
    ./stage.ps1 -Local   # a local branch with uncommitted work, for the local-branch tour
