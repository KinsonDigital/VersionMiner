// <copyright file="IntegrationTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.IO.Abstractions;
using FluentAssertions;
using Octokit;
using VersionMiner;
using VersionMiner.Services;

namespace VersionMinerIntegrationTests;

/// <summary>
/// Performs various integration tests of the GitHub action.
/// </summary>
public class IntegrationTests : IntegrationTestsBase, IDisposable
{
    private const string RepoToken = "DO-NOT-COMMIT-TOKEN";
    private readonly GitHubAction action;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTests"/> class.
    /// </summary>
    public IntegrationTests()
    {
        var githubClient = new GitHubClient(new ProductHeaderValue("version-miner-testing"));

        var repoContentClient = githubClient.Repository.Content;
        var repoFileDataService = new RepoFileDataService(repoContentClient);
        var consoleService = new GitHubConsoleService();
        var parserService = new XMLParserService();
        var envVarService = new EnvVarService();
        var fileSystem = new FileSystem();
        var file = fileSystem.File;
        var actionOutputService = new ActionOutputService(envVarService, file);

        this.action = new GitHubAction(
            consoleService,
            githubClient,
            repoFileDataService,
            parserService,
            actionOutputService);
    }

    [Fact]
    public async void Run_WhenEverythingExists_ReturnsSuccess()
    {
        // Arrange
        Exception? possibleException = null;

        var inputs = CreateInputs();

        // Act
        await this.action.Run(
            inputs,
            () => { },
            e => possibleException = e);

        // Assert
        Assert.True(possibleException is null, $"Exception Thrown:{Environment.NewLine}\t{possibleException?.Message ?? string.Empty}");
    }

    [Theory]
    [InlineData(nameof(ActionInputs.RepoName), "does-not-exist-repo", "Not Found")]
    [InlineData(nameof(ActionInputs.BranchName), "does-not-exist-branch", "Branch not found")]
    [InlineData(nameof(ActionInputs.FilePath), "invalid-file-path", "The file 'invalid-file-path' in the repository 'ActionTestRepo' for the owner 'KinsonDigital' was not found.")]
    [InlineData(nameof(ActionInputs.FileFormat), "invalid-format", "The 'file-format' value of 'invalid-format' is invalid.\r\nThe only file format currently supported is XML.")]
    [InlineData(nameof(ActionInputs.FileFormat), "", "The 'file-format' value of '' is invalid.\r\nThe only file format currently supported is XML.")]
    [InlineData(nameof(ActionInputs.VersionKeys), "", "No version keys supplied for the 'version-keys' input.")]
    public async void Run_WithInvalidInputValue_ReturnsCorrectInvalidResult(
        string inputName,
        string inputValue,
        string expectedExMessage)
    {
        // Arrange
        var onErrorInvoked = false;
        var inputs = CreateInputs();
        inputs.SetPropertyValue(inputName, inputValue);

        // Act
        await this.action.Run(
            inputs,
            () =>
            {
                // Assert
                onErrorInvoked.Should().BeTrue("onError() was not invoked.");
            },
            e =>
            {
                // Assert
                onErrorInvoked = true;
                Assert.NotNull(e);
                e.Message.Should().Be(expectedExMessage);
            });
    }

    /// <inheritdoc/>
    public void Dispose() => this.action.Dispose();

    /// <summary>
    /// Creates a new instance of <see cref="ActionInputs"/> with default values for the purpose of testing.
    /// </summary>
    private ActionInputs CreateInputs(
        string repoOwner = "KinsonDigital",
        string repoName = "ActionTestRepo",
        string branchName = "version-miner-testing",
        string filePath = "sample-project-file.csproj",
        string fileFormat = "xml",
        string versionKeys = "Version,FileVersion",
        bool caseSensitiveKeys = true,
        string trimStartFromBranch = "refs/heads/",
        bool failOnKeyValueMismatch = true,
        bool failWhenVersionNotFound = true)
        => new ()
        {
            RepoToken = RepoToken,
            RepoOwner = repoOwner,
            RepoName = repoName,
            BranchName = branchName,
            FilePath = filePath,
            FileFormat = fileFormat,
            VersionKeys = versionKeys,
            CaseSensitiveKeys = caseSensitiveKeys,
            TrimStartFromBranch = trimStartFromBranch,
            FailOnKeyValueMismatch = failOnKeyValueMismatch,
            FailWhenVersionNotFound = failWhenVersionNotFound,
        };
}
