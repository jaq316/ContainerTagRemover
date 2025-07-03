# Usage Examples

## Basic Usage with Keep Tags

Here are some examples of how to use the `--keep-tags` feature:

### 1. Keep specific tags from command line

```bash
# Keep latest and stable tags in addition to the policy defined in config.json
containertagremover https://myregistry.azurecr.io myimage config.json --keep-tags latest,stable
```

### 2. Keep version-specific tags

```bash
# Keep specific versions that might be important for rollback
containertagremover https://myregistry.azurecr.io myimage config.json --keep-tags 1.0.0,1.5.0,2.0.0
```

### 3. Combined usage with output file

```bash
# Keep tags and save results to a file
containertagremover https://myregistry.azurecr.io myimage config.json --keep-tags latest,stable --output-file results.json
```

### 4. Configuration file with keep tags

Create a config.json file:
```json
{
  "Major": 2,
  "Minor": 2,
  "KeepTags": ["latest", "stable", "main"]
}
```

Then run:
```bash
# This will keep both the tags specified in config.json and the command line
containertagremover https://myregistry.azurecr.io myimage config.json --keep-tags v1.0.0,production
```

## How it works

The tool will:
1. Apply the Major/Minor version policy to determine which semantic versions to keep
2. Keep any tags specified in the configuration file's `KeepTags` array
3. Keep any tags specified via the `--keep-tags` command line argument
4. Remove all other tags

Tags specified in `KeepTags` (either in config or command line) are kept regardless of whether they follow semantic versioning or not.
