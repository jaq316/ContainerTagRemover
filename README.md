# Container Tag Remover

Container Tag Remover is a .NET tool written in C# to remove old image tags from container registries. It supports Dockerhub and Azure Container Registry and can be easily extended to support new container registries in the future. The tool determines which tags to remove based on SemVer and a configuration file that specifies the number of tags to keep for Major and Minor versions.

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

The configuration file is a JSON file that specifies the number of tags to keep for Major and Minor versions. Example structure:

```json
{
  "Major": 5,
  "Minor": 10
}
```

If the configuration file is not specified, the tool will use the default values: Major: 2, Minor: 2.

## Environment Variables

To authenticate with Dockerhub and Azure Container Registry, you need to set the following environment variables:

### Dockerhub

* `DOCKERHUB_USERNAME`: Your Dockerhub username.
* `DOCKERHUB_PASSWORD`: Your Dockerhub password.

Example:

```sh
export DOCKERHUB_USERNAME=your-username
export DOCKERHUB_PASSWORD=your-password
```

### Azure Container Registry

* `AZURE_TENANT_ID`: Your Azure tenant ID.
* `AZURE_CLIENT_ID`: Your Azure client ID.
* `AZURE_CLIENT_SECRET`: Your Azure client secret (optional, if not set, `DefaultAzureCredential` will be used).

Example:

```sh
export AZURE_TENANT_ID=your-tenant-id
export AZURE_CLIENT_ID=your-client-id
export AZURE_CLIENT_SECRET=your-client-secret
```

## Building and Running the Tool

1. Clone the repository:

```sh
git clone https://github.com/jaq316/ContainerTagRemover.git
cd src/ContainerTagRemover
```

2. Build the solution:

```sh
dotnet build
```

3. Run the tool:

```sh
dotnet run --project src/ContainerTagRemover/ContainerTagRemover.csproj -- <registry-url> <image> <config-file> [--output-file <output-file>]
```

Replace `<registry-url>`, `<image>`, and `<config-file>` with the appropriate values. Optionally, specify `<output-file>` to output the list of removed and kept tags to a JSON file.

If the configuration file is not specified, the tool will use the default values: Major: 2, Minor: 2.

If any of the required arguments are not provided, the tool will prompt you to enter them during execution.

## Running the Tests

1. Navigate to the test project directory:

```sh
cd tests/ContainerTagRemover.Tests
```

2. Run the tests:

```sh
dotnet test
```

## Installing the Tool as a .NET Global Tool

To install the Container Tag Remover as a .NET global tool, use the following command:

```sh
dotnet tool install --global --add-source ./nupkg containertagremover
```

## Using the Tool as a .NET Global Tool

Once installed, you can use the tool from any directory by running:

```sh
containertagremover <registry-url> <image> <config-file> [--output-file <output-file>]
```

Replace `<registry-url>`, `<image>`, and `<config-file>` with the appropriate values. Optionally, specify `<output-file>` to output the list of removed and kept tags to a JSON file.

If the configuration file is not specified, the tool will use the default values: Major: 2, Minor: 2.

If any of the required arguments are not provided, the tool will prompt you to enter them during execution.

## GitVersion and Release Generation

This project uses GitVersion to generate release versions. GitVersion is a tool that generates version numbers based on your Git history. It follows Semantic Versioning (SemVer) principles and can be configured to suit your versioning strategy.

### Release Generation Process

1. Whenever the main branch is updated, the GitHub workflow will trigger the release generation process.
2. GitVersion will be used to determine the version number based on the commit history.
3. A new release will be created with the generated version number.

### Configuration

The GitVersion configuration file (`gitversion.yml`) is included in the repository and can be customized to fit your versioning strategy. For more information on configuring GitVersion, refer to the [GitVersion documentation](https://gitversion.net/docs/).

### GitHub Workflow

The GitHub workflow file (`.github/workflows/build-and-test.yml`) has been updated to include steps for installing and using GitVersion to generate release versions. The workflow will automatically create a new release whenever the main branch is updated.
