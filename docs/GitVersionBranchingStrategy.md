# GitVersion Branching Strategy

This repository uses GitVersion for versioning and release management. GitVersion follows the principles of Semantic Versioning (SemVer) to determine the version number based on the Git commit history.

## Branching Strategy

The branching strategy used in this repository is as follows:

* **Main Branch**: The `main` branch is the primary branch where the stable codebase resides. All releases are made from this branch.
* **Feature Branches**: Feature branches are created from the `main` branch for developing new features. The naming convention for feature branches is `feature/<feature-name>`.
* **Bugfix Branches**: Bugfix branches are created from the `main` branch for fixing bugs. The naming convention for bugfix branches is `bugfix/<bugfix-name>`.
* **Release Branches**: Release branches are created from the `main` branch for preparing a new release. The naming convention for release branches is `release/<version>`.
* **Hotfix Branches**: Hotfix branches are created from the `main` branch for urgent fixes to the production codebase. The naming convention for hotfix branches is `hotfix/<hotfix-name>`.

## Using GitVersion

To use GitVersion for generating version numbers, follow these steps:

1. Install GitVersion:

```sh
dotnet tool install -g GitVersion.Tool
```

2. Run GitVersion in the root directory of the repository:

```sh
gitversion
```

This will output the version number based on the current Git commit history.

## Release Generation

The release generation process involves the following steps:

1. Update the version number using GitVersion.
2. Build the solution.
3. Run the tests.
4. Package the tool.
5. Publish the tool to NuGet.
6. Create a GitHub release.

These steps are automated using GitHub Actions. The workflow file `.github/workflows/build-and-test.yml` defines the steps to build, test, package, and publish the tool, as well as create a GitHub release.

### GitHub Actions Workflow

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
