// <copyright file="IntegrationTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Octokit;
using VersionMiner;
using VersionMiner.Services;
using MinerHttpClient = VersionMiner.Services.HttpClient;

namespace VersionMinerIntegrationTests;

/// <summary>
/// Performs various integration tests of the GitHub action.
/// </summary>
public class IntegrationTests : IDisposable
{
    private const string RepoToken = "";
    private readonly GitHubAction action;
    private readonly IHttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTests"/> class.
    /// </summary>
    public IntegrationTests()
    {
        this.httpClient = new MinerHttpClient();
        var githubClient = new GitHubClient(new ProductHeaderValue("version-miner-testing"));
        var repoFileDataService = new RepoFileDataService(this.httpClient);
        var consoleService = new GitHubConsoleService();
        var parserService = new XMLParserService();
        var actionOutputService = new ActionOutputService(consoleService);

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
    [InlineData(nameof(ActionInputs.RepoName), "does-not-exist-repo", "The repository 'does-not-exist-repo' does not exist.")]
    [InlineData(nameof(ActionInputs.BranchName), "does-not-exist-branch", "Branch not found")]
    [InlineData(nameof(ActionInputs.FilePath), "invalid-file-path", "Request failed with status code '404(NotFound)' for file 'invalid-file-path'.")]
    [InlineData(nameof(ActionInputs.FileFormat), "invalid-format", "The 'file-format' value of 'invalid-format' is invalid.\r\nThe only file format currently supported is XML.")]
    [InlineData(nameof(ActionInputs.FileFormat), "", "The 'file-format' value of '' is invalid.\r\nThe only file format currently supported is XML.")]
    [InlineData(nameof(ActionInputs.VersionKeys), "", "No version keys supplied for the 'version-keys' input.")]
    public async void Run_WithNoVersionKeysInputValue_ReturnsCorrectInvalidResult(
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
    public void Dispose()
    {
        this.action.Dispose();
        this.httpClient.Dispose();
    }

    /// <summary>
    /// Creates a new instance of <see cref="ActionInputs"/> with default values for the purpose of testing.
    /// </summary>
    private static ActionInputs CreateInputs(
        string repoToken = RepoToken,
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
            RepoToken = repoToken,
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
