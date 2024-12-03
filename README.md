# Container Tag Remover

Container Tag Remover is a .NET tool written in C# to remove old image tags from container registries. It supports Dockerhub and Azure Container Registry and can be easily extended to support new container registries in the future. The tool determines which tags to remove based on SemVer and a configuration file that specifies the number of tags to keep for Major, Minor, and Patch versions.

## Features

- Supports Dockerhub and Azure Container Registry
- Easily extensible to support new container registries
- Removes old image tags based on SemVer and configuration file
- Follows SOLID principles
- 100% code coverage in tests
- Uses xUnit as the test framework
- Uses Moq as the mocking library
- Uses Shouldly for assertions
- Tests are in a separate project from the main tool code

## Configuration File

The configuration file is a JSON file that specifies the number of tags to keep for Major, Minor, and Patch versions. Example structure:

```json
{
  "Major": 5,
  "Minor": 10,
  "Patch": 20
}
```

## Building and Running the Tool

1. Clone the repository:

```sh
git clone https://github.com/githubnext/workspace-blank.git
cd workspace-blank
```

2. Build the solution:

```sh
dotnet build
```

3. Run the tool:

```sh
dotnet run --project src/ContainerTagRemover/ContainerTagRemover.csproj -- <registry> <repository> <config-file>
```

Replace `<registry>`, `<repository>`, and `<config-file>` with the appropriate values.

## Running the Tests

1. Navigate to the test project directory:

```sh
cd tests/ContainerTagRemover.Tests
```

2. Run the tests:

```sh
dotnet test
```
