// <copyright file="RepoFileDataServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Net;
using FluentAssertions;
using Moq;
using VersionMiner;
using VersionMiner.Exceptions;
using VersionMiner.Services;
using VersionMinerTests.Helpers;

namespace VersionMinerTests.Services;

/// <summary>
/// Tests the <see cref="RepoFileDataService"/> class.
/// </summary>
public class RepoFileDataServiceTests
{
    private const string BaseUrl = "https://api.github.com";
    private readonly Mock<IHttpClient> mockClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoFileDataServiceTests"/> class.
    /// </summary>
    public RepoFileDataServiceTests()
    {
        this.mockClient = new Mock<IHttpClient>();
        this.mockClient.SetupGet(p => p.BaseUrl).Returns(BaseUrl);
    }

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
    public async void GetFileData_WhenHttpResponseIsNull_ThrowException()
    {
        // Arrange
        const string repoOwner = "test-owner";
        const string repoName = "test-repo";
        const string branchName = "test-branch";
        const string filePath = "invalid-path";
        const string queryString = $"?ref={branchName}";
        const string requestUri = $@"/repos/{repoOwner}/{repoName}/contents/{filePath}{queryString}";

        var service = CreateService();

        // Act
        var act = () => service.GetFileData(repoOwner, repoName, branchName, filePath);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage($"The response from '{this.mockClient.Object.BaseUrl}/{requestUri}' returned null.");
    }

    [Fact]
    public async void GetFileData_WhenFileIsNotFound_ThrowsException()
    {
        // Arrange
        const string repoOwner = "test-owner";
        const string repoName = "test-repo";
        const string branchName = "test-branch";
        const string filePath = "invalid-path";

        var expectedMsg =
            $"Request failed with status code '{(int)HttpStatusCode.NotFound}({HttpStatusCode.NotFound})' for file 'invalid-path'.";

        this.mockClient.Setup(m => m.ExecuteGetAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponse { StatusCode = HttpStatusCode.NotFound });
        var service = CreateService();

        // Act
        var act = () => service.GetFileData(repoOwner, repoName, branchName, filePath);

        // Assert
        await act.Should().ThrowAsync<FileNotFoundException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public async void GetFileData_WhenOtherHttpError_ThrowsException()
    {
        // Arrange
        const string repoOwner = "test-owner";
        const string repoName = "test-repo";
        const string branchName = "test-branch";
        const string filePath = "invalid-path";

        var expectedMsg =
            $"Request failed with status code '{(int)HttpStatusCode.Forbidden}({HttpStatusCode.Forbidden})'.";

        this.mockClient.Setup(m => m.ExecuteGetAsync(It.IsAny<string>()))
            .ReturnsAsync(new HttpResponse { StatusCode = HttpStatusCode.Forbidden });
        var service = CreateService();

        // Act
        var act = () => service.GetFileData(repoOwner, repoName, branchName, filePath);

        // Assert
        await act.Should().ThrowAsync<HttpRequestException>()
            .WithMessage(expectedMsg);
    }

    [Fact]
    public async void GetFileData_WhenFileExists_ReturnsFileData()
    {
        // Arrange
        const string repoOwner = "test-owner";
        const string repoName = "test-repo";
        const string branchName = "test-branch";
        const string filePath = "invalid-path";
        const string queryString = $"?ref={branchName}";
        const string requestUri = $@"/repos/{repoOwner}/{repoName}/contents/{filePath}{queryString}";
        const string expectedFileData = "test-data";

        this.mockClient.Setup(m => m.ExecuteGetAsync(requestUri))
            .ReturnsAsync(new HttpResponse { Content = expectedFileData, StatusCode = HttpStatusCode.OK });

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

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfService()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.Dispose();
        service.Dispose();

        // Assert
        this.mockClient.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="RepoFileDataService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private RepoFileDataService CreateService() => new (this.mockClient.Object);
}
