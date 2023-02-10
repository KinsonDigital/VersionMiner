// <copyright file="RepoFileDataServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Text;
using FluentAssertions;
using Moq;
using Octokit;
using VersionMiner.Exceptions;
using VersionMiner.Services;

namespace VersionMinerTests.Services;

/// <summary>
/// Tests the <see cref="RepoFileDataService"/> class.
/// </summary>
public class RepoFileDataServiceTests
{
    private readonly Mock<IRepositoryContentsClient> mockRepoContentClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoFileDataServiceTests"/> class.
    /// </summary>
    public RepoFileDataServiceTests() => this.mockRepoContentClient = new Mock<IRepositoryContentsClient>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullClientParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RepoFileDataService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'client')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void AuthToken_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.AuthToken = "test-token";

        // Assert
        service.AuthToken.Should().Be("test-token");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async void GetFileData_WhenInvokedWithNullRepoOwner_ThrowsException(string repoOwner)
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = () => service.GetFileData(
            repoOwner,
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>());

        // Assert
        await act.Should().ThrowAsync<NullOrEmptyStringException>()
            .WithMessage("The param 'repoOwner' cannot be null or empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async void GetFileData_WhenInvokedWithNullOrEmptyRepoName_ThrowsException(string repoName)
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = () => service.GetFileData(
            "test-owner",
            repoName,
            "test-branch",
            "test-path");

        // Assert
        await act.Should().ThrowAsync<NullOrEmptyStringException>()
            .WithMessage("The param 'repoName' cannot be null or empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async void GetFileData_WhenInvokedWithNullOrEmptyBranchName_ThrowsException(string branchName)
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = () => service.GetFileData(
            "test-owner",
            "repo-name",
            branchName,
            "test-path");

        // Assert
        await act.Should().ThrowAsync<NullOrEmptyStringException>()
            .WithMessage("The param 'branchName' cannot be null or empty.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async void GetFileData_WhenInvokedWithNullOrEmptyFilePath_ThrowsException(string filePath)
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = () => service.GetFileData(
            "test-owner",
            "repo-name",
            "test-branch",
            filePath);

        // Assert
        await act.Should().ThrowAsync<NullOrEmptyStringException>()
            .WithMessage("The param 'filePath' cannot be null or empty.");
    }

    [Fact]
    public async void GetFileData_WhenContentIsNotFound_ThrowException()
    {
        // Arrange
        const string repoOwner = "test-owner";
        const string repoName = "test-repo";
        const string branchName = "test-branch";
        const string filePath = "invalid-path";
        const string expected = $"The file '{filePath}' in the repository '{repoName}' for the owner '{repoOwner}' was not found.";

        var clientResult = Array.Empty<RepositoryContent>().AsReadOnly();

        this.mockRepoContentClient
            .Setup(m => m.GetAllContentsByRef(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(clientResult);

        var service = CreateService();

        // Act
        var act = () => service.GetFileData(repoOwner, repoName, branchName, filePath);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(expected);
    }

    [Fact]
    public async void GetFileData_WhenFileExists_ReturnsFileData()
    {
        // Arrange
        const string repoOwner = "test-owner";
        const string repoName = "test-repo";
        const string branchName = "test-branch";
        const string filePath = "invalid-path";
        const string expectedFileData = "test-data";

        var expected = new[]
        {
            CreateRepositoryContent(filePath, expectedFileData),
        }.AsReadOnly();

        this.mockRepoContentClient
            .Setup(m => m.GetAllContentsByRef(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(expected);

        var service = CreateService();

        // Act
        var actual = string.Empty;
        var act = async () =>
        {
            actual = await service.GetFileData(repoOwner, repoName, branchName, filePath);
        };

        // Assert
        await act.Should().NotThrowAsync();
        actual.Should().NotBeNullOrEmpty();
        actual.Should().Be(expectedFileData);
    }
    #endregion

    /// <summary>
    /// Creates a new <see cref="RepositoryContent"/> object for the purpose of testing.
    /// </summary>
    /// <param name="path">The path to the content relative to the root of the repository.</param>
    /// <param name="encodedContent">The encoded content of the file.</param>
    /// <returns>The repository content.</returns>
    private static RepositoryContent CreateRepositoryContent(string path, string encodedContent)
    {
        return new RepositoryContent(
            name: string.Empty, // string
            path: path, // string
            sha: string.Empty, // string
            size: 0, // int
            type: ContentType.File, // ContentType
            downloadUrl: string.Empty, // string
            url: string.Empty, // string
            gitUrl: string.Empty, // string
            htmlUrl: string.Empty, // string
            encoding: "base64", // string
            encodedContent: Convert.ToBase64String(Encoding.ASCII.GetBytes(encodedContent)), // string
            target: string.Empty, // string
            submoduleGitUrl: string.Empty); // string
    }

    /// <summary>
    /// Creates a new instance of <see cref="RepoFileDataService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private RepoFileDataService CreateService() => new (this.mockRepoContentClient.Object);
}
