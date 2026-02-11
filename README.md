# TO Integrations Tests

## Overview

This project contains integration tests for the TO Integrations API using SpecFlow (BDD) and NUnit.

## Project Structure

```
to-integrations-tests/
├── Config/                     # Configuration classes
│   ├── AppConfig.cs           # Main configuration loader
│   ├── DirectionContext.cs    # Current direction context
│   ├── DirectionProvider.cs   # Direction/URL provider
│   └── ToIntegrationsEnvironment.cs  # Environment settings
├── CRUD/                       # API interaction layer
│   └── [EntityName]/          # Entity-specific folder
│       ├── [EntityName]Presetup.cs   # Default test data
│       └── Api/
│           ├── [EntityName]Api.cs    # HTTP calls
│           └── [EntityName]Crud.cs   # Response handling
├── Features/                   # SpecFlow feature files
│   └── [EntityName]/
│       ├── [EntityName].feature      # BDD scenarios
│       └── steps/
│           └── [EntityName]Steps.cs  # Step definitions
├── HelperMethods/             # Shared utilities
│   ├── CommonResponseSteps.cs # Common Then-steps
│   ├── ExecuteForAllDirections.cs
│   ├── HelperMethods.cs       # Date helpers, etc.
│   └── LastResponse.cs        # Response time tracking
├── Docs/                      # Documentation
├── Atata.test.json           # Test configuration
├── SpecFlowHooks.cs          # SpecFlow lifecycle hooks
└── to-integrations.csproj    # Project file
```

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or Rider
- SpecFlow extension for your IDE

## Configuration

Edit `Atata.test.json` to configure:
- API credentials (AgentId, AgentPassword)
- Direction URLs for different environments

## Running Tests

```bash
# Restore packages
dotnet restore

# Build
dotnet build

# Run all tests
dotnet test

# Run specific feature
dotnet test --filter "FullyQualifiedName~EntityName"
```

## Adding New Test Cases

1. Create folder structure in `CRUD/[EntityName]/`
2. Create `[EntityName]Presetup.cs` with test data
3. Create `Api/[EntityName]Api.cs` for HTTP calls
4. Create `Api/[EntityName]Crud.cs` for response handling
5. Create `Features/[EntityName]/[EntityName].feature`
6. Create `Features/[EntityName]/steps/[EntityName]Steps.cs`
7. Update `HelperMethods/CommonResponseSteps.cs` with new CRUD

## Suggestions for a good README

Every project is different, so consider which of these sections apply to yours. The sections used in the template are suggestions for most open source projects. Also keep in mind that while a README can be too long and detailed, too long is better than too short. If you think your README is too long, consider utilizing another form of documentation rather than cutting out information.

## Name
Choose a self-explaining name for your project.

## Description
Let people know what your project can do specifically. Provide context and add a link to any reference visitors might be unfamiliar with. A list of Features or a Background subsection can also be added here. If there are alternatives to your project, this is a good place to list differentiating factors.

## Badges
On some READMEs, you may see small images that convey metadata, such as whether or not all the tests are passing for the project. You can use Shields to add some to your README. Many services also have instructions for adding a badge.

## Visuals
Depending on what you are making, it can be a good idea to include screenshots or even a video (you'll frequently see GIFs rather than actual videos). Tools like ttygif can help, but check out Asciinema for a more sophisticated method.

## Installation
Within a particular ecosystem, there may be a common way of installing things, such as using Yarn, NuGet, or Homebrew. However, consider the possibility that whoever is reading your README is a novice and would like more guidance. Listing specific steps helps remove ambiguity and gets people to using your project as quickly as possible. If it only runs in a specific context like a particular programming language version or operating system or has dependencies that have to be installed manually, also add a Requirements subsection.

## Usage
Use examples liberally, and show the expected output if you can. It's helpful to have inline the smallest example of usage that you can demonstrate, while providing links to more sophisticated examples if they are too long to reasonably include in the README.

## Support
Tell people where they can go to for help. It can be any combination of an issue tracker, a chat room, an email address, etc.

## Roadmap
If you have ideas for releases in the future, it is a good idea to list them in the README.

## Contributing
State if you are open to contributions and what your requirements are for accepting them.

For people who want to make changes to your project, it's helpful to have some documentation on how to get started. Perhaps there is a script that they should run or some environment variables that they need to set. Make these steps explicit. These instructions could also be useful to your future self.

You can also document commands to lint the code or run tests. These steps help to ensure high code quality and reduce the likelihood that the changes inadvertently break something. Having instructions for running tests is especially helpful if it requires external setup, such as starting a Selenium server for testing in a browser.

## Authors and acknowledgment
Show your appreciation to those who have contributed to the project.

## License
For open source projects, say how it is licensed.

## Project status
If you have run out of energy or time for your project, put a note at the top of the README saying that development has slowed down or stopped completely. Someone may choose to fork your project or volunteer to step in as a maintainer or owner, allowing your project to keep going. You can also make an explicit request for maintainers.
