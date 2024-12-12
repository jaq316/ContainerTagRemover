# GitHub Release Generation

The GitHub release generation process involves the following steps:

1. Update the version number using GitVersion.
2. Build the solution.
3. Run the tests.
4. Package the tool.
5. Publish the tool to NuGet.
6. Create a GitHub release.

Note: DockerHub implementation is experimental.

These steps are automated using GitHub Actions. The workflow file `.github/workflows/build-and-test.yml` defines the steps to build, test, package, and publish the tool, as well as create a GitHub release.

## GitHub Actions Workflow

The GitHub Actions workflow file `.github/workflows/build-and-test.yml` includes the following steps:

1. Checkout the repository.
2. Set up .NET.
3. Install GitVersion.
4. Use GitVersion to generate the version number.
5. Restore dependencies.
6. Build the solution.
7. Run the tests.
8. Package the tool.
9. Publish the tool to NuGet (if not a pull request).
10. Create a GitHub release (if not a pull request).

The workflow is triggered on pushes to the `main` branch and on pull requests targeting the `main` branch.

Note: The GitHub workflow now prevents execution on changes to the `docs` folder and its contents.
