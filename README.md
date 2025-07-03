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

The configuration file is a JSON file that specifies the number of tags to keep for Major and Minor versions, and optionally a list of specific tags to keep. Example structure:

```json
{
  "Major": 5,
  "Minor": 10,
  "KeepTags": ["latest", "stable", "1.0.0"]
}
```

- **Major**: Number of major versions to keep (default: 2)
- **Minor**: Number of minor versions to keep per major version (default: 2)
- **KeepTags**: Array of specific tags to keep regardless of the Major/Minor policy (optional)

### Tag Retention Priority

Tags are retained based on the following priority:

1. **Explicit Keep Tags**: Any tag specified in the configuration file's `KeepTags` array or via the `--keep-tags` command line parameter will always be kept
2. **Semantic Version Policy**: Among semantic version tags, the tool keeps the latest versions according to the Major/Minor configuration
3. **Non-Semantic Tags**: Non-semantic version tags (like `latest`, `stable`, `dev`) are only kept if explicitly specified in `KeepTags`

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
containertagremover <registry-url> <image> <config-file> [--output-file <output-file>] [--keep-tags <tag1,tag2,...>]
```

Replace `<registry-url>`, `<image>`, and `<config-file>` with the appropriate values. 

Optional parameters:
- `--output-file <output-file>`: Output the list of removed and kept tags to a JSON file
- `--keep-tags <tag1,tag2,...>`: Comma-separated list of tags to keep (in addition to those specified in the configuration file)

Examples:

```sh
# Basic usage
containertagremover https://myregistry.azurecr.io myimage config.json

# With output file
containertagremover https://myregistry.azurecr.io myimage config.json --output-file results.json

# With specific tags to keep
containertagremover https://myregistry.azurecr.io myimage config.json --keep-tags latest,stable,v1.0.0

# With both output file and keep tags
containertagremover https://myregistry.azurecr.io myimage config.json --output-file results.json --keep-tags latest,stable
```

If the configuration file is not specified, the tool will use the default values: Major: 2, Minor: 2.

If any of the required arguments are not provided, the tool will prompt you to enter them during execution.

## Command Line Options

### Required Arguments

- `<registry-url>`: The URL of the container registry (e.g., `https://myregistry.azurecr.io` or `https://dockerhub`)
- `<image>`: The name of the container image
- `<config-file>`: Path to the JSON configuration file

### Optional Arguments

- `--output-file <output-file>`: Path to save the results as a JSON file containing lists of removed and kept tags
- `--keep-tags <tag1,tag2,...>`: Comma-separated list of tags to keep in addition to those specified in the configuration file

### Tag Retention Logic

The tool applies the following logic to determine which tags to keep:

1. **Semantic Version Policy**: Keeps the specified number of major versions and minor versions per major version based on the configuration file
2. **Configuration File Tags**: Keeps any tags listed in the `KeepTags` array in the configuration file
3. **Command Line Tags**: Keeps any tags specified via the `--keep-tags` parameter
4. **Tag Merging**: Tags from the configuration file and command line are merged (additive, not replaced)

Tags specified in `KeepTags` (either in config or command line) are preserved regardless of whether they follow semantic versioning.

### Note

Azure container registry image names must be lowercase.

## Contributing

We welcome contributions to this project! Please see the [Contributing Guidelines](CONTRIBUTING.md) for more information on how to get started.
