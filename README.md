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
