# Running the Tests

To run the tests for the Container Tag Remover tool, follow these steps:

1. Ensure you have the .NET SDK installed on your machine. You can download it from the [.NET website](https://dotnet.microsoft.com/download).

2. Open a terminal or command prompt.

3. Navigate to the `tests/ContainerTagRemover.Tests` directory.

4. Run the following command to execute the tests:

```sh
dotnet test
```

This will run all the tests in the `ContainerTagRemover.Tests` project and display the test results in the terminal or command prompt.

## Test Frameworks and Libraries

The Container Tag Remover tool uses the following test frameworks and libraries:

- **xUnit**: The main test framework used for writing and running tests.
- **Moq**: A mocking library used to create mock objects for testing.
- **Shouldly**: An assertion library used for writing readable and expressive assertions in tests.

## Code Coverage

The Container Tag Remover tool aims for 100% code coverage in tests. Code coverage is measured using the `coverlet.collector` package, which is integrated with the `dotnet test` command.

To generate a code coverage report, run the following command:

```sh
dotnet test --collect:"XPlat Code Coverage"
```

The code coverage report will be generated in the `TestResults` directory in the project root. You can open the `coverage.cobertura.xml` file in a code coverage visualization tool to view the detailed code coverage report.
