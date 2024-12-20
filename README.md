# Container Tag Remover

Container Tag Remover is a .NET tool written in C# to remove old image tags from container registries. It supports Dockerhub and Azure Container Registry and can be easily extended to support new container registries in the future. The tool determines which tags to remove based on SemVer and a configuration file that specifies the number of tags to keep for Major and Minor versions.

## Features

- Supports Dockerhub (experimental) and Azure Container Registry
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

To authenticate with Azure Container Registry and Dockerhub, you need to set the following environment variables:

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

### Dockerhub

* `DOCKERHUB_USERNAME`: Your Dockerhub username.
* `DOCKERHUB_PASSWORD`: Your Dockerhub password.

Note: Dockerhub implementation is experimental.

Example:

```sh
export DOCKERHUB_USERNAME=your-username
export DOCKERHUB_PASSWORD=your-password
```

## Running the Tool

To run the Container Tag Remover tool, follow these steps:

1. Install the tool as a .NET global tool:

```sh
dotnet tool install --global ContainerTagRemover
```

2. Run the tool using the following command:

```sh
containertagremover <registry-url> <image> <config-file> [--output-file <output-file>]
```

Replace `<registry-url>`, `<image>`, and `<config-file>` with the appropriate values. Optionally, specify `<output-file>` to output the list of removed and kept tags to a JSON file.

If the configuration file is not specified, the tool will use the default values: Major: 2, Minor: 2.

If any of the required arguments are not provided, the tool will prompt you to enter them during execution.

### Note

Azure container registry image names must be lowercase.

## Contributing

We welcome contributions to this project! Please see the [Contributing Guidelines](CONTRIBUTING.md) for more information on how to get started.
