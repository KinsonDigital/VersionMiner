// <copyright file="ActionInputTests.cs" company="KinsonDigital">
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
public class ActionInputTests
{
    #region Prop Tests
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
            .AssertOptionAttrProps("version-keys", "The key(s) that can possibly hold the version in the file.");

        inputs.CaseSensitiveKeys.Should().BeTrue();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.CaseSensitiveKeys)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.CaseSensitiveKeys))
            .AssertOptionAttrProps("case-sensitive-keys", false, "If true, then the key searching will be case sensitive.");

        inputs.FailOnMissingKey.Should().BeTrue();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.FailOnMissingKey)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.FailOnMissingKey))
            .AssertOptionAttrProps("fail-on-missing-key", false, "If true, will fail if any of the keys described in the list of 'version-keys' are missing.");

        inputs.FailOnKeyValueMismatch.Should().BeFalse();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.FailOnKeyValueMismatch)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.FailOnKeyValueMismatch))
            .AssertOptionAttrProps("fail-on-key-value-mismatch", false, "If true, will fail the action if all of the key values listed in the 'version-keys' input do not match.");

        inputs.FailWhenVersionNotFound.Should().BeTrue();
        typeof(ActionInputs).GetProperty(nameof(ActionInputs.FailWhenVersionNotFound)).Should().BeDecoratedWith<OptionAttribute>();
        inputs.GetAttrFromProp<OptionAttribute>(nameof(ActionInputs.FailWhenVersionNotFound))
            .AssertOptionAttrProps("fail-when-version-not-found", false, "If true, the action will fail if the version is not found.");
    }

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
        var actual = inputs.RepoName;

        // Assert
        actual.Should().Be("test-repo");
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
    public void FileType_WhenSettingValue_ReturnsCorrectResult()
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

    [Fact]
    public void CaseSensitiveKeys_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();
        var expectedValue = !inputs.CaseSensitiveKeys;

        // Act
        inputs.CaseSensitiveKeys = expectedValue;
        var actual = inputs.CaseSensitiveKeys;

        // Assert
        actual.Should().Be(expectedValue);
    }

    [Fact]
    public void FailOnMissingKey_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();
        var expectedValue = !inputs.FailOnMissingKey;

        // Act
        inputs.FailOnMissingKey = expectedValue;
        var actual = inputs.FailOnMissingKey;

        // Assert
        actual.Should().Be(expectedValue);
    }

    [Fact]
    public void FailOnKeyValueMismatch_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();
        var expectedValue = !inputs.FailOnKeyValueMismatch;

        // Act
        inputs.FailOnKeyValueMismatch = expectedValue;
        var actual = inputs.FailOnKeyValueMismatch;

        // Assert
        actual.Should().Be(expectedValue);
    }

    [Fact]
    public void FailWhenVersionNotFound_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var inputs = new ActionInputs();
        var expectedValue = !inputs.FailWhenVersionNotFound;

        // Act
        inputs.FailWhenVersionNotFound = expectedValue;
        var actual = inputs.FailWhenVersionNotFound;

        // Assert
        actual.Should().Be(expectedValue);
    }
    #endregion
}
