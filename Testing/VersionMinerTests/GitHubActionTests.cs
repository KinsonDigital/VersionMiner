// <copyright file="GitHubActionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Moq;
using Octokit;
using VersionMiner;
using VersionMiner.Exceptions;
using VersionMiner.Services;
using VersionMinerTests.Helpers;

namespace VersionMinerTests;

/// <summary>
/// Tests the <see cref="GitHubAction"/> class.
/// </summary>
public class GitHubActionTests
{
    private const long RepoId = 123456789;
    private const string AuthToken = "test-token";
    private const string RepoName = "test-repo";
    private const string BranchName = "test-branch";
    private const string VersionOutputName = "version";
    private const string XMLVersionTagName = "Version";
    private const string XMLFileVersionTagName = "FileVersion";
    private const string XMLFileFormat = "xml";
    private readonly Mock<IGitHubClient> mockGitHubClient;
    private readonly Mock<IGitHubConsoleService> mockConsoleService;
    private readonly Mock<IRepositoryBranchesClient> mockRepoBranchesClient;
    private readonly Mock<IRepoFileDataService> mockRepoFileDataService;
    private readonly Mock<IDataParserService> mockXMLParserService;
    private readonly Mock<IActionOutputService> mockActionOutputService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubActionTests"/> class.
    /// </summary>
    public GitHubActionTests()
    {
        this.mockRepoBranchesClient = new Mock<IRepositoryBranchesClient>();
        this.mockRepoBranchesClient.Setup(m => m.Get(It.IsAny<long>(), It.IsAny<string>()))
            .ReturnsAsync(new Branch());
        var mockReposClient = new Mock<IRepositoriesClient>();
        mockReposClient.SetupGet(p => p.Branch).Returns(this.mockRepoBranchesClient.Object);

        var repo = new Repository();
        repo.SetPropValue(nameof(repo.Name), RepoName);
        repo.SetPropValue(nameof(repo.Id), RepoId);

        mockReposClient.Setup(m => m.GetAllForCurrent())
            .ReturnsAsync(new[] { repo });

        var mockConnection = new Mock<IConnection>();
        mockConnection.SetupProperty(p => p.Credentials);

        this.mockGitHubClient = new Mock<IGitHubClient>();
        this.mockGitHubClient.Setup(m => m.Repository).Returns(mockReposClient.Object);
        this.mockGitHubClient.SetupGet(p => p.Connection).Returns(mockConnection.Object);

        this.mockConsoleService = new Mock<IGitHubConsoleService>();

        this.mockRepoFileDataService = new Mock<IRepoFileDataService>();
        this.mockRepoFileDataService.Setup(m =>
                m.GetFileData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        this.mockXMLParserService = new Mock<IDataParserService>();
        this.mockActionOutputService = new Mock<IActionOutputService>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullConsoleServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new GitHubAction(
                null,
                this.mockGitHubClient.Object,
                this.mockRepoFileDataService.Object,
                this.mockXMLParserService.Object,
                this.mockActionOutputService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'consoleService')");
    }

    [Fact]
    public void Ctor_WithNullGitHubClientParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new GitHubAction(
                this.mockConsoleService.Object,
                null,
                this.mockRepoFileDataService.Object,
                this.mockXMLParserService.Object,
                this.mockActionOutputService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'githubClient')");
    }

    [Fact]
    public void Ctor_WithNullRepoFileDataServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new GitHubAction(
                this.mockConsoleService.Object,
                this.mockGitHubClient.Object,
                null,
                this.mockXMLParserService.Object,
                this.mockActionOutputService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'repoFileDataService')");
    }

    [Fact]
    public void Ctor_WithNullXMLParserServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new GitHubAction(
                this.mockConsoleService.Object,
                this.mockGitHubClient.Object,
                this.mockRepoFileDataService.Object,
                null,
                this.mockActionOutputService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'xmlParserService')");
    }

    [Fact]
    public void Ctor_WithNullActionOutputServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new GitHubAction(
                this.mockConsoleService.Object,
                this.mockGitHubClient.Object,
                this.mockRepoFileDataService.Object,
                this.mockXMLParserService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'actionOutputService')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public async void Run_WhenInvoked_ShowsWelcomeMessage()
    {
        // Arrange
        var expectedUrl = "https://github.com/KinsonDigital/VersionMiner/issues/new/choose";
        var inputs = CreateInputs(versionKeys: "Version");

        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        this.mockConsoleService.VerifyOnce(m => m.WriteLine("Welcome to Version Miner!! 🪨⛏️"));
        this.mockConsoleService.VerifyOnce(m => m.WriteLine("A GitHub action for pulling versions out of various types of files."));
        this.mockConsoleService.VerifyOnce(m => m.WriteLine($"To open an issue, click here 👉🏼 {expectedUrl}"));
    }

    [Fact]
    public async void Run_WhenInvoked_ShowsWelcomeMessageInCorrectOrder()
    {
        // Arrange
        var expectedOrder = new List<string>
        {
            $"{nameof(IGitHubConsoleService.WriteLine)}|Welcome to Version Miner!! 🪨⛏️",
            $"{nameof(IGitHubConsoleService.WriteLine)}|A GitHub action for pulling versions out of various types of files.",
            nameof(IGitHubConsoleService.BlankLine),
            nameof(IGitHubConsoleService.BlankLine),
        };
        var actualExecutionOrder = new List<string>();
        var inputs = CreateInputs(versionKeys: "Version");

        this.mockConsoleService.Setup(m => m.WriteLine(It.IsAny<string>()))
            .Callback<string>(value => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.WriteLine)}|{value}"));
        this.mockConsoleService.Setup(m => m.BlankLine())
            .Callback(() => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.BlankLine)}"));
        this.mockConsoleService.Setup(m => m.StartGroup(It.IsAny<string>()))
            .Callback<string>(value => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.StartGroup)}|{value}"));
        this.mockConsoleService.Setup(m => m.EndGroup())
            .Callback(() => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.EndGroup)}"));

        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        actualExecutionOrder.Should().ContainInOrder(expectedOrder);
    }

    [Fact]
    public async void Run_WithTrimStartFromBranchSetToValue_TrimsBranchName()
    {
        // Arrange
        const string branchBeforeTrim = "refs/heads/test-branch";
        const string branchAfterTrim = "test-branch";
        var inputs = CreateInputs(
            versionKeys: "Version",
            branchName: branchBeforeTrim,
            trimStartFromBranch: "refs/heads/");
        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        this.mockConsoleService.VerifyOnce(m => m.WriteLine($"Branch Before Trimming: {branchBeforeTrim}"));
        this.mockConsoleService.VerifyOnce(m
            => m.WriteLine($"The text '{inputs.TrimStartFromBranch}' has been trimmed from the branch name."));
        this.mockConsoleService.VerifyOnce(m => m.WriteLine($"Branch After Trimming: {branchAfterTrim}"));
        this.mockConsoleService.Verify(m => m.BlankLine(), Times.Exactly(8));
    }

    [Theory]
    [InlineData("xml")]
    [InlineData("XML")]
    public async void Run_WithInvalidFileFormat_ThrowsException(string fileFormat)
    {
        // Arrange
        var inputs = CreateInputs(versionKeys: "Version", fileFormat: fileFormat);
        inputs.FileFormat = "wrong-type";
        var action = CreateAction();

        var expectedMsg = $"The 'file-format' value of '{inputs.FileFormat}' is invalid.";
        expectedMsg += $"{Environment.NewLine}The only file format currently supported is XML.";

        // Act & Assert
        void Assert(Exception e)
        {
            e.Should().BeOfType<InvalidFileFormatException>();
            e.Message.Should().Be(expectedMsg);
        }

        await action.Run(inputs, () => { }, Assert);
    }

    [Fact]
    public async void Run_WhenRepoNameDoesNotExist_ThrowsException()
    {
        // Arrange
        var inputs = CreateInputs(repoName: "other-repo");
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<HttpRequestException>()
            .WithMessage("The repository 'other-repo' does not exist.");
    }

    [Fact]
    public async void Run_WhenBranchNameDoesNotExist_ThrowsException()
    {
        // Arrange
        // this.mockReposClient.Setup(m => m.GetAllForCurrent())
        //     .ReturnsAsync(() =>
        //     {
        //         return new[] { new Repository(RepoId) };
        //     });
        this.mockRepoBranchesClient.Setup(m => m.Get(RepoId, BranchName))
            .ReturnsAsync(null as Branch);

        var inputs = CreateInputs(It.IsAny<string>());
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<HttpRequestException>()
            .WithMessage("The repository branch 'test-branch' does not exist.");
    }

    [Fact]
    public async void Run_WhenAllTagValuesDoNotMatch_ThrowsException()
    {
        // Arrange
        var expectedExceptionMsg = "All values must match.";
        expectedExceptionMsg += "This failure only occurs if the 'fail-on-key-value-mismatch' action input is set to 'true'.";

        const string testData = "test-data";
        this.mockRepoFileDataService.Setup(m
                => m.GetFileData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(testData);

        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLFileVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLVersionTagName, true))
            .Returns("1.2.3");
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLFileVersionTagName, true))
            .Returns("3.2.1");

        var inputs = CreateInputs(versionKeys: "Version,FileVersion");
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<ValuesMismatchException>()
            .WithMessage(expectedExceptionMsg);
        this.mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLVersionTagName, true));
        this.mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLFileVersionTagName, true));
        this.mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLVersionTagName, true));
        this.mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLFileVersionTagName, true));
        this.mockActionOutputService.VerifyNever(m => m.SetOutputValue(It.IsAny<string>(), It.IsAny<string>()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(null)]
    public async void Run_WhenInputFailOnKeyValueMisMatchIsFalseOrNull_DoesNotThrowException(bool? failOnKeyValueMismatch)
    {
        // Arrange
        var inputs = CreateInputs(versionKeys: "Version,FileVersion", failOnKeyValueMismatch: failOnKeyValueMismatch);
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, _ => { });

        // Assert
        await act.Should().NotThrowAsync<ValuesMismatchException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(",")]
    [InlineData(" ,")]
    [InlineData(", ")]
    [InlineData("  ")]
    public async void Run_WhenNoVersionKeysParsed_ThrowsException(string versionKeys)
    {
        // Arrange
        this.mockRepoFileDataService.Setup(m
                => m.GetFileData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("test-data");

        var inputs = CreateInputs(versionKeys: versionKeys,
            failOnKeyValueMismatch: false,
            failWhenVersionNotFound: false);
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<NoVersionFoundException>()
            .WithMessage("No version keys supplied for the 'version-keys' input.");
    }

    [Theory]
    [InlineData("Version,FileVersion", true)]
    [InlineData("Version, FileVersion", true)]
    [InlineData("Version ,FileVersion", true)]
    [InlineData("Version , FileVersion", true)]
    [InlineData("Version   ,      FileVersion", true)]
    [InlineData("Version   ,", true)]
    [InlineData(", FileVersion", true)]
    [InlineData("Version", true)]
    [InlineData("version", false)]
    [InlineData("version", null)]
    public async void Run_WithDifferentVersionKeys_ProperlyParsesKeys(
        string versionKeys,
        bool? isCaseSensitive)
    {
        // Arrange
        var keys = versionKeys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var inputs = CreateInputs(versionKeys: versionKeys,
            failOnKeyValueMismatch: false,
            failWhenVersionNotFound: false,
            caseSensitiveKeys: isCaseSensitive ?? false);
        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        Assert.All(keys, key =>
        {
            this.mockXMLParserService.VerifyOnce(m => m.KeyExists(It.IsAny<string>(), key, isCaseSensitive ?? false));
        });
    }

    [Fact]
    public async void Run_WithVersionTagIsEmptyAndFileVersionTagIsNotEmpty_GetsFileVersionTagValue()
    {
        // Arrange
        const string expectedVersion = "1.2.3";
        const string testData = "test-data";
        this.mockRepoFileDataService.Setup(m
                => m.GetFileData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(testData);

        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLFileVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLVersionTagName, true))
            .Returns(string.Empty);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLFileVersionTagName, true))
            .Returns(expectedVersion);

        var inputs = CreateInputs(versionKeys: "Version,FileVersion",
            failOnKeyValueMismatch: false);
        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        this.mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLVersionTagName, true));
        this.mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLFileVersionTagName, true));
        this.mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLVersionTagName, true));
        this.mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLFileVersionTagName, true));
        this.mockActionOutputService.VerifyOnce(m => m.SetOutputValue(VersionOutputName, expectedVersion));
        this.mockConsoleService.VerifyOnce(m => m.StartGroup("Version Miner Outputs"));
        this.mockConsoleService.VerifyOnce(m => m.WriteLine($"version: {expectedVersion}"));
        this.mockConsoleService.VerifyOnce(m => m.EndGroup());

        this.mockConsoleService.VerifyOnce(m => m.Write($"✔️️Verifying if the repository '{inputs.RepoName}' exists . . . ", false));
        this.mockConsoleService.VerifyOnce(m => m.Write("the repository exists.", true));

        this.mockConsoleService.VerifyOnce(m => m.Write($"✔️️Verifying if the repository branch '{inputs.BranchName}' exists . . . ", false));
        this.mockConsoleService.VerifyOnce(m => m.Write("the branch exists.", true));

        this.mockConsoleService.VerifyOnce(m => m.Write($"✔️️Getting data for file '{inputs.FilePath}' . . . ", false));
        this.mockConsoleService.VerifyOnce(m => m.Write("data retrieved", true));

        this.mockConsoleService.VerifyOnce(m => m.Write("✔️️Validating version keys . . . ", false));
        this.mockConsoleService.VerifyOnce(m => m.Write("version keys validated.", true));

        this.mockConsoleService.VerifyOnce(m => m.Write("✔️️Pulling version from file . . . ", false));
        this.mockConsoleService.VerifyOnce(m => m.Write("version pulled from file.", true));

        this.mockConsoleService.Verify(m => m.BlankLine(), Times.Exactly(8));
    }

    [Fact]
    public async void Run_WhenNoVersionExists_AND_WhenInputFailWhenVersionNotFoundIsSetToTrue_ThrowsException()
    {
        // Arrange
        var expectedExceptionMsg = "No version value was found.";
        expectedExceptionMsg += $"{Environment.NewLine}If you do not want the GitHub action to fail when no version is found,";
        expectedExceptionMsg += "set the 'fail-when-version-not-found' input to a value of 'false'.";

        this.mockXMLParserService.Setup(m => m.KeyExists(It.IsAny<string>(), XMLVersionTagName, true))
            .Returns(true);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(It.IsAny<string>(), XMLVersionTagName, true))
            .Returns(string.Empty);
        var inputs = CreateInputs(versionKeys: "Version",
            failWhenVersionNotFound: true);
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<NoVersionFoundException>()
            .WithMessage(expectedExceptionMsg);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(null)]
    public async void Run_WhenNoVersionExists_AND_WhenInputFailWhenVersionNotFoundIsSetToNullOrFalse_DoesNotThrowException(
        bool? failWhenVersionNotFoundInput)
    {
        // Arrange
        this.mockXMLParserService.Setup(m => m.KeyExists(It.IsAny<string>(), XMLVersionTagName, true))
            .Returns(true);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(It.IsAny<string>(), XMLVersionTagName, true))
            .Returns(string.Empty);
        var inputs = CreateInputs(versionKeys: "Version",
            failWhenVersionNotFound: failWhenVersionNotFoundInput);
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, _ => { });

        // Assert
        await act.Should()
            .NotThrowAsync<NoVersionFoundException>();
    }

    [Fact]
    public async void Run_WhenAllKeysExistButAreEmptyForOnKeyValueMismatchInput_DoesNotFail()
    {
        // Arrange
        this.mockXMLParserService.Setup(m => m.KeyExists(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(true);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(string.Empty);

        var inputs = CreateInputs(versionKeys: "Version,FileVersion,AssemblyVersion",
            failOnKeyValueMismatch: true,
            failWhenVersionNotFound: false);
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, _ => { });

        // Assert
        await act.Should().NotThrowAsync("all requested versions that are empty still match.");
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfAction()
    {
        // Arrange
        var action = CreateAction();

        // Act
        action.Dispose();
        action.Dispose();

        // Assert
        this.mockRepoFileDataService.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ActionInputs"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private static ActionInputs CreateInputs(
        string repoOwner = "test-owner",
        string repoName = RepoName,
        string repoToken = AuthToken,
        string filePath = "test-path",
        string branchName = "test-branch",
        string fileFormat = XMLFileFormat,
        string versionKeys = "Version,FileVersion",
        bool caseSensitiveKeys = true,
        string trimStartFromBranch = "",
        bool? failOnKeyValueMismatch = true,
        bool? failWhenVersionNotFound = true) => new ()
    {
        RepoOwner = repoOwner,
        RepoName = repoName,
        RepoToken = repoToken,
        FilePath = filePath,
        BranchName = branchName,
        FileFormat = fileFormat,
        VersionKeys = versionKeys,
        CaseSensitiveKeys = caseSensitiveKeys,
        TrimStartFromBranch = trimStartFromBranch,
        FailOnKeyValueMismatch = failOnKeyValueMismatch,
        FailWhenVersionNotFound = failWhenVersionNotFound,
    };

    /// <summary>
    /// Creates a new instance of <see cref="GitHubAction"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GitHubAction CreateAction()
        => new (
            this.mockConsoleService.Object,
            this.mockGitHubClient.Object,
            this.mockRepoFileDataService.Object,
            this.mockXMLParserService.Object,
            this.mockActionOutputService.Object);
}
