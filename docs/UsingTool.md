# Using the Tool as a .NET Global Tool

Once you have installed the Container Tag Remover tool as a .NET global tool, you can use it from any terminal or command prompt. Follow these steps to use the tool:

1. Open a terminal or command prompt.

2. Run the tool using the following command:

```sh
containertagremover <registry-url> <image> <config-file> [--output-file <output-file>]
```

Replace `<registry-url>`, `<image>`, and `<config-file>` with the appropriate values. Optionally, specify `<output-file>` to output the list of removed and kept tags to a JSON file.

3. The tool will prompt you to enter any missing arguments during execution.

4. The tool will authenticate with the specified container registry and remove old image tags based on the configuration file.

5. The tool will display the list of removed and kept tags in the terminal or command prompt.

6. If you specified an output file, the tool will write the list of removed and kept tags to the specified JSON file.
