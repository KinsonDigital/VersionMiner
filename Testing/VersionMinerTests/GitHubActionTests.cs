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

public class GitHubActionTests
{
    private const string VersionOutputName = "version";
    private const string XMLVersionTagName = "Version";
    private const string XMLFileVersionTagName = "FileVersion";
    private const string XMLFileType = "xml";
    private readonly Mock<IGitHubConsoleService> _mockConsoleService;
    private readonly Mock<IGitHubDataService> _mockDataService;
    private readonly Mock<IDataParserService> _mockXMLParserService;
    private readonly Mock<IActionOutputService> _mockActionOutputService;

    public GitHubActionTests()
    {
        _mockConsoleService = new Mock<IGitHubConsoleService>();

        _mockDataService = new Mock<IGitHubDataService>();
        _mockDataService.Setup(m => m.OwnerExists())
            .ReturnsAsync(() => true);
        _mockDataService.Setup(m => m.RepoExists())
            .ReturnsAsync(() => true);
        _mockDataService.Setup(m => m.BranchExists())
            .ReturnsAsync(() => true);
        _mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(() => string.Empty);

        _mockXMLParserService = new Mock<IDataParserService>();
        _mockActionOutputService = new Mock<IActionOutputService>();
    }

    #region Method Tests
    [Fact]
    public async void Run_WhenInvoked_ShowsWelcomeMessage()
    {
        // Arrange
        var inputs = CreateInputs("Version");

        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        _mockConsoleService.VerifyOnce(m => m.WriteLine("Welcome to Version Miner!! 🪨⛏️"));
        _mockConsoleService.VerifyOnce(m => m.WriteLine("A GitHub action for pulling versions out of various types of files!!"));
    }

    [Fact]
    public async void Run_WhenInvoked_ShowsWelcomeMessageInCorrectOrder()
    {
        // Arrange
        var expectedOrder = new List<string>()
        {
            $"{nameof(IGitHubConsoleService.WriteLine)}|Welcome to Version Miner!! 🪨⛏️",
            $"{nameof(IGitHubConsoleService.WriteLine)}|A GitHub action for pulling versions out of various types of files!!",
            nameof(IGitHubConsoleService.BlankLine),
            nameof(IGitHubConsoleService.BlankLine),
        };
        var actualExecutionOrder = new List<string>();
        var inputs = CreateInputs("Version");

        _mockConsoleService.Setup(m => m.WriteLine(It.IsAny<string>()))
            .Callback<string>(value => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.WriteLine)}|{value}"));
        _mockConsoleService.Setup(m => m.BlankLine())
            .Callback(() => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.BlankLine)}"));
        _mockConsoleService.Setup(m => m.StartGroup(It.IsAny<string>()))
            .Callback<string>(value => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.StartGroup)}|{value}"));
        _mockConsoleService.Setup(m => m.EndGroup())
            .Callback(() => actualExecutionOrder.Add($"{nameof(IGitHubConsoleService.EndGroup)}"));

        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        actualExecutionOrder.Should()
            .ContainInOrder(expectedOrder);
    }

    [Fact]
    public async void Run_WhenInvoked_SetsDataServiceProps()
    {
        // Arrange
        _mockDataService.SetupSet(p => p.RepoOwner = It.IsAny<string>());
        _mockDataService.SetupSet(p => p.RepoName = It.IsAny<string>());
        _mockDataService.SetupSet(p => p.BranchName = It.IsAny<string>());
        _mockDataService.SetupSet(p => p.FilePath = It.IsAny<string>());
        var inputs = CreateInputs("Version");
        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        _mockDataService.VerifySetOnce(p => p.RepoOwner = "test-owner");
        _mockDataService.VerifySetOnce(p => p.RepoName = "test-name");
        _mockDataService.VerifySetOnce(p => p.BranchName = "test-branch");
        _mockDataService.VerifySetOnce(p => p.FilePath = "test-path");
    }

    [Fact]
    public async void Run_WithInvalidFileType_ThrowsException()
    {
        // Arrange
        var inputs = CreateInputs("Version");
        inputs.FileFormat = "wrong-type";
        var action = CreateAction();

        var expectedMsg = $"The file type value of '{inputs.FileFormat}' is invalid.";
        expectedMsg += $"{Environment.NewLine}The only file type currently supported are csproj files.";

        // Act & Assert
        void Assert(Exception e)
        {
            e.Should().BeOfType<InvalidFileTypeException>();
            e.Message.Should().Be(expectedMsg);
        }

        await action.Run(inputs, () => { }, Assert);
    }

    [Fact]
    public async void Run_WhenRepoOwnerDoesNotExist_ThrowsException()
    {
        // Arrange
        _mockDataService.Setup(m => m.OwnerExists()).ReturnsAsync(false);

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
        _mockDataService.Setup(m => m.RepoExists()).ReturnsAsync(false);

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
        _mockDataService.Setup(m => m.BranchExists()).ReturnsAsync(false);

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
        _mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(testData);
        _mockXMLParserService.Setup(m => m.KeyExists(testData, XMLVersionTagName, true)).Returns(true);
        _mockXMLParserService.Setup(m => m.KeyExists(testData, XMLFileVersionTagName, true)).Returns(true);
        _mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLVersionTagName, true))
            .Returns("1.2.3");
        _mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLFileVersionTagName, true))
            .Returns("3.2.1");

        var inputs = CreateInputs("Version,FileVersion");
        var action = CreateAction();

        // Act
        var act = () => action.Run(inputs, () => { }, e => throw e);

        // Assert
        await act.Should()
            .ThrowAsync<ValuesMismatchException>()
            .WithMessage(expectedExceptionMsg);
        _mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLVersionTagName, true));
        _mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLFileVersionTagName, true));
        _mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLVersionTagName, true));
        _mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLFileVersionTagName, true));
        _mockActionOutputService.VerifyNever(m => m.SetOutputValue(It.IsAny<string>(), It.IsAny<string>()));
    }

    [Theory]
    [InlineData("")]
    [InlineData(",")]
    [InlineData(" ,")]
    [InlineData(", ")]
    [InlineData("  ")]
    public async void Run_WithVersionKeysParsed_ThrowsException(string versionKeys)
    {
        // Arrange
        const string testData = "test-data"; // TODO: Remove
        _mockDataService.Setup(m => m.GetFileData())
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
    [InlineData("Version,FileVersion")]
    [InlineData("Version, FileVersion")]
    [InlineData("Version ,FileVersion")]
    [InlineData("Version , FileVersion")]
    [InlineData("Version   ,      FileVersion")]
    [InlineData("Version   ,")]
    [InlineData(", FileVersion")]
    [InlineData("Version")]
    public async void Run_WithDifferentVersionKeys_ProperlyParsesKeys(string versionKeys)
    {
        // Arrange
        var keys = versionKeys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        const string testData = "test-data"; // TODO: Remove
        _mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(testData);

        var inputs = CreateInputs(versionKeys,
            failOnKeyValueMismatch: false,
            failWhenVersionNotFound: false);
        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        Assert.All(keys, key =>
        {
            _mockXMLParserService.VerifyOnce(m => m.KeyExists(It.IsAny<string>(), key, true));
        });
    }

    [Fact]
    public async void Run_WithVersionTagIsEmptyAndFileVersionTagIsNotEmpty_GetsFileVersionTagValue()
    {
        // Arrange
        const string expectedVersion = "1.2.3";
        const string testData = "test-data";
        _mockDataService.Setup(m => m.GetFileData())
            .ReturnsAsync(testData);
        _mockXMLParserService.Setup(m => m.KeyExists(testData, XMLVersionTagName, true)).Returns(true);
        _mockXMLParserService.Setup(m => m.KeyExists(testData, XMLFileVersionTagName, true)).Returns(true);
        _mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLVersionTagName, true))
            .Returns(string.Empty);
        _mockXMLParserService.Setup(m => m.GetKeyValue(testData, XMLFileVersionTagName, true))
            .Returns(expectedVersion);

        var inputs = CreateInputs("Version,FileVersion",
            failOnKeyValueMismatch: false);
        var action = CreateAction();

        // Act
        await action.Run(inputs, () => { }, _ => { });

        // Assert
        _mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLVersionTagName, true));
        _mockXMLParserService.VerifyOnce(m => m.KeyExists(testData, XMLFileVersionTagName, true));
        _mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLVersionTagName, true));
        _mockXMLParserService.VerifyOnce(m => m.GetKeyValue(testData, XMLFileVersionTagName, true));
        _mockActionOutputService.VerifyOnce(m => m.SetOutputValue(VersionOutputName, expectedVersion));
        _mockConsoleService.VerifyOnce(m => m.StartGroup("Version Miner Outputs"));
        _mockConsoleService.VerifyOnce(m => m.WriteLine($"version: {expectedVersion}"));
        _mockConsoleService.VerifyOnce(m => m.EndGroup());

        _mockConsoleService.VerifyOnce(m => m.Write($"✔️️Verifying if the repository owner '{inputs.RepoOwner}' exists . . . ", false));
        _mockConsoleService.VerifyOnce(m => m.Write("the owner exists.", true));

        _mockConsoleService.VerifyOnce(m => m.Write($"✔️️Verifying if the repository '{inputs.RepoName}' exists . . . ", false));
        _mockConsoleService.VerifyOnce(m => m.Write("the repository exists.", true));

        _mockConsoleService.VerifyOnce(m => m.Write($"✔️️Verifying if the repository branch '{inputs.BranchName}' exists . . . ", false));
        _mockConsoleService.VerifyOnce(m => m.Write("the branch exists.", true));

        _mockConsoleService.VerifyOnce(m => m.Write($"✔️️Getting data for file '{inputs.FilePath}' . . . ", false));
        _mockConsoleService.VerifyOnce(m => m.Write("data retrieved", true));

        _mockConsoleService.VerifyOnce(m => m.Write("✔️️Validating version keys . . . ", false));
        _mockConsoleService.VerifyOnce(m => m.Write("version keys validated.", true));

        _mockConsoleService.VerifyOnce(m => m.Write("✔️️Pulling version from file . . . ", false));
        _mockConsoleService.VerifyOnce(m => m.Write("version pulled from file.", true));

        _mockConsoleService.Verify(m => m.BlankLine(), Times.Exactly(9));
    }

    [Fact]
    public async void Run_WhenNoVersionIsFoundForFailWhenVersionNotFoundInputAndIsSetToTrue_ThrowsException()
    {
        // Arrange
        var expectedExceptionMsg = "No version value was found.";
        expectedExceptionMsg += $"{Environment.NewLine}If you do not want the GitHub action to fail when no version is found,";
        expectedExceptionMsg += "set the 'fail-when-version-not-found' input to a value of 'false'.";

        _mockXMLParserService.Setup(m => m.KeyExists(It.IsAny<string>(), XMLVersionTagName, true)).Returns(true);
        _mockXMLParserService.Setup(m => m.GetKeyValue(It.IsAny<string>(), XMLVersionTagName, true)).Returns(string.Empty);
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

    [Fact]
    public async void Run_WhenAllKeysExistButAreEmptyForOnKeyValueMismatchInput_DoesNotFail()
    {
        // Arrange
        _mockXMLParserService.Setup(m => m.KeyExists(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .Returns(true);
        _mockXMLParserService.Setup(m => m.GetKeyValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
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
        _mockDataService.VerifyOnce(m => m.Dispose());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ActionInputs"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private static ActionInputs CreateInputs(string versionKeys,
        bool failOnKeyValueMismatch = true,
        bool failWhenVersionNotFound = true) => new ()
    {
        RepoOwner = "test-owner",
        RepoName = "test-name",
        BranchName = "test-branch",
        FilePath = "test-path",
        FileFormat = XMLFileType,
        VersionKeys = versionKeys,
        FailOnKeyValueMismatch = failOnKeyValueMismatch,
        FailWhenVersionNotFound = failWhenVersionNotFound,
    };

    /// <summary>
    /// Creates a new instance of <see cref="GitHubAction"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GitHubAction CreateAction()
        => new (
            _mockConsoleService.Object,
            _mockDataService.Object,
            _mockXMLParserService.Object,
            _mockActionOutputService.Object);
}
