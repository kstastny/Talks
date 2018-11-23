- title : F# FAKE
- description : FAKE quick overview
- author : Karel Šťastný
- theme : moon
- transition : default

***

## F# MAKE - FAKE

<br />
<br />

#### A DSL for build tasks and more 

<br />
<br />
Karel Šťastný

***
### FAKE
* What is FAKE?
* Why FAKE?
* Basic Usage
* Extensibility
* Alternatives

' presentation is mostly for .NET developers
' I will skip or gloss over some less important parts

***
### What is FAKE?
* DSL for build tasks
* versions
    * FAKE 4
    * FAKE 5


' explain DSL - domain specific language
' define the domain here - building and deploying applications (won't be talking about Docker etc...)
' i.e. checkout, build, test, prepare release (zip, msi, exe, deb,...), report results
' cross platform

***

### Why FAKE?

#### Why build scripts
* You can't setup everything in CI/CD build tool
* easier versioning, change tracking
* builds run independently, you don't need build server

' CI/CD I will compare mostly with TeamCity, because that's what I know
' e.g. download and apply localization files from Lingohub
' source control - possible with TC but awkward and not regularly done
' no dependency on build server - easier to prepare builds locally, debug problems
' I am in no way suggesting, they should stop using CI servers!

---

### Why FAKE?

#### Why FAKE and not others?
* Powerful and easy to use
* Easily extensible
* Proven, mature project
    * <a href="https://fake.build/help-users.html">FAKE users</a>
* Standardize on .NET code in your toolchain
* you get to use F# ;-)

' Powerful for it's purpose, you can do anything
' Akka.NET, Octokit.NET, SolrNET, ElasticSearch.NET
' re F#: easily usable even for C# projects, we did that without any problems (replaced NAnt)

***

### Basic Usage

    // include Fake lib
    #r @"packages/FAKE/tools/FakeLib.dll"
    open Fake

    // Default target
    Target "Default" (fun _ ->
        trace "Hello World from FAKE"
    )

    // start build
    RunTargetOrDefault "Default"

' Target - one part of script that you want to run, "function"
' you can define dependencies between targets (will show later)

---

> Example - Build Application

' show dependeny specification, running target - what is output
' clean build directory, build and copy files
' run application

---

> Example - Parameterize build

' build.cmd Build output=../out

---

> Example - Install Windows Service

' copy files - FileIncludes - same as NAnt's fileset http://nant.sourceforge.net/release/0.92/help/types/fileset.html
' run as admin, task DeployWindowsService
' BUT - not really supported


***

### Extensibility

* FAKE runs on .NET -  whatever you can write in .NET, you can use in FAKE

---

### F# Extension

> Example - Libraryscript module

---

### C# Extension

* reuse already written code
* easier for non-F# developers

> Example - Library DLL

***
### Alternatives

* NAnt
* MSBuild
* <a href="https://cakebuild.net/">Cake</a>
* <a href="https://github.com/psake/psake">PSake</a>
* <a href="https://github.com/ruby/rake">Rake</a>
* <a href="https://github.com/chucknorris/uppercut">UppercuT</a>

' just choose any letter, append -ake and that's your alternative :)

***
### The Good

* Easy to use
* Plenty of support <a href="https://fake.build/apidocs/index.html">out of the box</a>
* Extensible
* F#
* You get to learn something new

---

### The Bad

* You have to learn something new
* For uninitiated, F# error messages might feel somewhat cryptic
    * This helps: https://fsharpforfunandprofit.com/troubleshooting-fsharp/ 

' something new - in the context of most .NET devs

---
### The Ugly

* No reassuring braces {}
* FAKE 5 Documentation is not newbie friendly (yet)
* FAKE 4 Documentation is a bit hard to find (Google still works)

***

<br />
<br />

### Q & A

<br />
<br />

***
### Sources

* https://fake.build/
* http://forki.github.io/FAKE.Intro/#/
* http://blog.2mas.xyz/take-control-of-your-build-ci-and-deployment-with-fsharp-fake/
* https://fsharpforfunandprofit.com/posts/low-risk-ways-to-use-fsharp-at-work-2/#fake
