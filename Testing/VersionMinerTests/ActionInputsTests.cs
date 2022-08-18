// <copyright file="ActionInputsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using CommandLine;
using FluentAssertions;
using VersionMiner;
using VersionMinerTests.Helpers;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable PossibleMultipleEnumeration
namespace VersionMinerTests;

/// <summary>
/// Tests the <see cref="ActionInputs"/> class.
/// </summary>
public class ActionInputsTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenConstructed_PropsHaveCorrectDefaultValuesAndDecoratedWithAttributes()
    {
        // Arrange & Act
        var inputs = new ActionInputs();

        // Assert
        inputs.RepoOwner.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.RepoOwner)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.RepoOwner))
            .AssertOptionAttrProps("repo-owner", true, "Gets or sets the owner of the repository.");

        inputs.RepoName.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.RepoName)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.RepoName))
            .AssertOptionAttrProps("repo-name", true, "Gets or sets the name of the repository.");

        inputs.RepoToken.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.RepoToken)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.RepoToken))
            .AssertOptionAttrProps("repo-token", false, "The GitHub or PAT token used to authenticate to the repository.");

        inputs.BranchName.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.BranchName)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.BranchName))
            .AssertOptionAttrProps("branch-name", true, "Gets or sets the name of the branch where the file lives.");

        inputs.FilePath.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.FilePath)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.FilePath))
            .AssertOptionAttrProps("file-path", true, "Gets or sets the path to the file relative to the root of the repository.");

        inputs.FileFormat.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.FileFormat)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.FileFormat))
            .AssertOptionAttrProps("file-format", true, "The format of the data in the file that contains the version.  Currently the only supported format is 'xml' and is not case sensitive.");

        inputs.VersionKeys.Should().NotBeNull();
        inputs.VersionKeys.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.VersionKeys)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.VersionKeys))
            .AssertOptionAttrProps("version-keys", "The key(s) that can hold the version in the file.");

        inputs.CaseSensitiveKeys.Should().BeTrue();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.CaseSensitiveKeys)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.CaseSensitiveKeys))
            .AssertOptionAttrProps("case-sensitive-keys", false, true, "If true, the key search will be case sensitive.");

        inputs.TrimStartFromBranch.Should().BeEmpty();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.TrimStartFromBranch)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.TrimStartFromBranch))
            .AssertOptionAttrProps("trim-start-from-branch", false, "Trims the start from the 'branch-name' value.");

        inputs.FailOnKeyValueMismatch.Should().BeFalse();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.FailOnKeyValueMismatch)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.FailOnKeyValueMismatch))
            .AssertOptionAttrProps("fail-on-key-value-mismatch", false, false, "If true, the action will fail if all of the key values in the list of 'version-keys' do not match.");

        inputs.FailWhenVersionNotFound.Should().BeTrue();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.FailWhenVersionNotFound)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.FailWhenVersionNotFound))
            .AssertOptionAttrProps("fail-when-version-not-found", false, true, "If true, the action will fail if the version is not found.");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void RepoOwner_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.RepoOwner = "test-owner";
        var actual = inputs.RepoOwner;

        // Assert
        actual.Should().Be("test-owner");
    }

    [Fact]
    public void RepoName_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.RepoName = "test-repo";

        // Assert
        inputs.RepoName.Should().Be("test-repo");
    }

    [Fact]
    public void RepoToken_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.RepoToken = "test-token";

        // Assert
        inputs.RepoToken.Should().Be("test-token");
    }

    [Fact]
    public void BranchName_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.BranchName = "test-branch";
        var actual = inputs.BranchName;

        // Assert
        actual.Should().Be("test-branch");
    }

    [Fact]
    public void FilePath_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.FilePath = "test-path";
        var actual = inputs.FilePath;

        // Assert
        actual.Should().Be("test-path");
    }

    [Fact]
    public void FileFormat_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.FileFormat = "test-file-format";
        var actual = inputs.FileFormat;

        // Assert
        actual.Should().Be("test-file-format");
    }

    [Fact]
    public void VersionKeys_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.VersionKeys = "Version,FileVersion";
        var actual = inputs.VersionKeys;

        // Assert
        actual.Should().Be("Version,FileVersion");
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(null, null)]
    public void CaseSensitiveKeys_WhenSettingValue_ReturnsCorrectResult(bool? value, bool? expected)
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.CaseSensitiveKeys = value;
        var actual = inputs.CaseSensitiveKeys;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void TrimStartFromBranch_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.TrimStartFromBranch = "test-value";

        // Assert
        inputs.TrimStartFromBranch.Should().Be("test-value");
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(null, null)]
    public void FailOnKeyValueMismatch_WhenSettingValue_ReturnsCorrectResult(bool? value, bool? expected)
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.FailOnKeyValueMismatch = value;
        var actual = inputs.FailOnKeyValueMismatch;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    [InlineData(null, null)]
    public void FailWhenVersionNotFound_WhenSettingValue_ReturnsCorrectResult(bool? value, bool? expected)
    {
        // Arrange
        var inputs = new ActionInputs();

        // Act
        inputs.FailWhenVersionNotFound = value;
        var actual = inputs.FailWhenVersionNotFound;

        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}
