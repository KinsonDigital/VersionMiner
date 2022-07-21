// <copyright file="GitHubActionTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Moq;
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
    private const string VersionOutputName = "version";
    private const string XMLVersionTagName = "Version";
    private const string XMLFileVersionTagName = "FileVersion";
    private const string XMLFileFormat = "xml";
    private readonly Mock<IGitHubConsoleService> mockConsoleService;
    private readonly Mock<IGitHubDataService> mockDataService;
    private readonly Mock<IDataParserService> mockXMLParserService;
    private readonly Mock<IActionOutputService> mockActionOutputService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubActionTests"/> class.
    /// </summary>
    public GitHubActionTests()
    {
        this.mockConsoleService = new Mock<IGitHubConsoleService>();

        this.mockDataService = new Mock<IGitHubDataService>();
        this.mockDataService.Setup(m => m.OwnerExists())
            .ReturnsAsync(() => true);
        this.mockDataService.Setup(m => m.RepoExists())
            .ReturnsAsync(() => true);
        this.mockDataService.Setup(m => m.BranchExists())
            .ReturnsAsync(() => true);
        this.mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(() => string.Empty);

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
                null, this.mockDataService.Object, this.mockXMLParserService.Object, this.mockActionOutputService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'consoleService')");
    }

    [Fact]
    public void Ctor_WithNullGitHubDataServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new GitHubAction(
                this.mockConsoleService.Object,
                null,
                this.mockXMLParserService.Object,
                this.mockActionOutputService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'gitHubDataService')");
    }

    [Fact]
    public void Ctor_WithNullXMLParserServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new GitHubAction(
                this.mockConsoleService.Object,
                this.mockDataService.Object,
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
                this.mockDataService.Object,
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
        var inputs = CreateInputs("Version");

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
        var expectedOrder = new List<string>()
        {
            $"{nameof(IGitHubConsoleService.WriteLine)}|Welcome to Version Miner!! 🪨⛏️",
            $"{nameof(IGitHubConsoleService.WriteLine)}|A GitHub action for pulling versions out of various types of files.",
            nameof(IGitHubConsoleService.BlankLine),
            nameof(IGitHubConsoleService.BlankLine),
        };
        var actualExecutionOrder = new List<string>();
        var inputs = CreateInputs("Version");

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
            "Version",
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
        this.mockConsoleService.Verify(m => m.BlankLine(), Times.Exactly(9));
    }

    [Fact]
    public async void Run_WhenInvoked_SetsDataServiceProps()
    {
        // Arrange
        this.mockDataService.SetupSet(p => p.RepoOwner = It.IsAny<string>());
        this.mockDataService.SetupSet(p => p.RepoName = It.IsAny<string>());
        this.mockDataService.SetupSet(p => p.BranchName = It.IsAny<string>());
        this.mockDataService.SetupSet(p => p.FilePath = It.IsAny<string>());
        var inputs = CreateInputs("Version");
        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        this.mockDataService.VerifySetOnce(p => p.RepoOwner = "test-owner");
        this.mockDataService.VerifySetOnce(p => p.RepoName = "test-name");
        this.mockDataService.VerifySetOnce(p => p.BranchName = "test-branch");
        this.mockDataService.VerifySetOnce(p => p.FilePath = "test-path");
    }

    [Fact]
    public async void Run_WithInvalidFileFormat_ThrowsException()
    {
        // Arrange
        var inputs = CreateInputs("Version");
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
    public async void Run_WhenRepoOwnerDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockDataService.Setup(m => m.OwnerExists()).ReturnsAsync(false);

        var inputs = CreateInputs(It.IsAny<string>());
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<OwnerDoesNotExistException>()
            .WithMessage("The repository owner 'test-owner' does not exist.");
    }

    [Fact]
    public async void Run_WhenRepoNameDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockDataService.Setup(m => m.RepoExists()).ReturnsAsync(false);

        var inputs = CreateInputs(It.IsAny<string>());
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<RepoDoesNotExistException>()
            .WithMessage("The repository 'test-name' does not exist.");
    }

    [Fact]
    public async void Run_WhenBranchNameDoesNotExist_ThrowsException()
    {
        // Arrange
        this.mockDataService.Setup(m => m.BranchExists()).ReturnsAsync(false);

        var inputs = CreateInputs(It.IsAny<string>());
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<BranchDoesNotExistException>()
            .WithMessage("The repository branch 'test-branch' does not exist.");
    }

    [Fact]
    public async void Run_WhenAllTagValuesDoNotMatch_ThrowsException()
    {
        // Arrange
        var expectedExceptionMsg = "All values must match.";
        expectedExceptionMsg += "This failure only occurs if the 'fail-on-key-value-mismatch' action input is set to 'true'.";

        const string testData = "test-data";
        this.mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(testData);
        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLFileVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLVersionTagName, true))
            .Returns("1.2.3");
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLFileVersionTagName, true))
            .Returns("3.2.1");

        var inputs = CreateInputs("Version,FileVersion");
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
        var inputs = CreateInputs("Version,FileVersion", failOnKeyValueMismatch: failOnKeyValueMismatch);
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
        const string testData = "test-data"; // TODO: Remove
        this.mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(testData);

        var inputs = CreateInputs(versionKeys,
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

        var inputs = CreateInputs(versionKeys,
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
        this.mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(testData);
        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.KeyExists(testData, XMLFileVersionTagName, true)).Returns(true);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLVersionTagName, true))
            .Returns(string.Empty);
        this.mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLFileVersionTagName, true))
            .Returns(expectedVersion);

        var inputs = CreateInputs("Version,FileVersion",
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

        this.mockConsoleService.VerifyOnce(m => m.Write($"✔️️Verifying if the repository owner '{inputs.RepoOwner}' exists . . . ", false));
        this.mockConsoleService.VerifyOnce(m => m.Write("the owner exists.", true));

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

        this.mockConsoleService.Verify(m => m.BlankLine(), Times.Exactly(9));
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
        var inputs = CreateInputs("Version",
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
        var inputs = CreateInputs("Version",
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

        var inputs = CreateInputs("Version,FileVersion,AssemblyVersion",
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
        this.mockDataService.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ActionInputs"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private static ActionInputs CreateInputs(
        string versionKeys,
        string branchName = "test-branch",
        bool? failOnKeyValueMismatch = true,
        bool? failWhenVersionNotFound = true,
        bool caseSensitiveKeys = true,
        string trimStartFromBranch = "") => new ()
    {
        RepoOwner = "test-owner",
        RepoName = "test-name",
        BranchName = branchName,
        FilePath = "test-path",
        FileFormat = XMLFileFormat,
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
        => new (this.mockConsoleService.Object, this.mockDataService.Object, this.mockXMLParserService.Object, this.mockActionOutputService.Object);
}
