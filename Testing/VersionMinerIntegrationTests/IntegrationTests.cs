// <copyright file="IntegrationTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using GitHubData;
using VersionMiner;
using VersionMiner.Services;
using GHHttpClient = GitHubData.HttpClient;

namespace VersionMinerIntegrationTests;

/// <summary>
/// Performs various integration tests of the GitHub action.
/// </summary>
public class IntegrationTests : IDisposable
{
    private readonly GitHubAction action;
    private readonly IHttpClient httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTests"/> class.
    /// </summary>
    public IntegrationTests()
    {
        this.httpClient = new GHHttpClient();
        var consoleService = new GitHubConsoleService();
        var gitHubDataService = new GitHubDataService(this.httpClient);
        var parserService = new XMLParserService();
        var actionOutputService = new ActionOutputService(consoleService);

        this.action = new GitHubAction(
            consoleService,
            gitHubDataService,
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
    [InlineData(nameof(ActionInputs.RepoOwner), "", "The 'repoOwner' value cannot be null or empty.")]
    [InlineData(nameof(ActionInputs.RepoName), "", "The 'repoName' value cannot be null or empty.")]
    [InlineData(nameof(ActionInputs.BranchName), "", "The 'branchName' value cannot be null or empty.")]
    [InlineData(nameof(ActionInputs.FilePath), "", "The 'filePath' value cannot be null or empty.")]
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
