// <copyright file="ExtensionMethodTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using FluentAssertions;
using VersionMiner;

namespace VersionMinerTests;

/// <summary>
/// Tests the extension methods.
/// </summary>
public class ExtensionMethodTests
{
    #region Method Tests
    [Theory]
    [InlineData(@"C:\", true)]
    [InlineData(@"C:\test-file.txt", true)]
    [InlineData(@"c:\", true)]
    [InlineData(@"c:/", true)]
    [InlineData(@"C:/", true)]
    [InlineData(@"C:/test-file.txt", true)]
    [InlineData(@":\", false)]
    [InlineData(@"C\", false)]
    [InlineData(@"C:", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void PathContainsDrive_WhenInvoked_ReturnsCorrectResult(string path, bool expected)
    {
        // Act
        var actual = path.PathContainsDrive();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("refs/heads/feature/123-my-branch", "refs/heads/", "feature/123-my-branch")]
    [InlineData("feature/123-branch-123", "123", "feature/123-branch-123")]
    [InlineData("feature/123-my-branch", "refs/heads/", "feature/123-my-branch")]
    [InlineData("feature/123-my-branch", null, "feature/123-my-branch")]
    [InlineData("feature/123-my-branch", "", "feature/123-my-branch")]
    public void TrimStart_WhenInvoked_ReturnsCorrectResult(
        string thisStr,
        string value,
        string expected)
    {
        // Act
        var actual = thisStr.TrimStart(value);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToKeyValuePairs_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[] { new KeyValuePair<string, int>("key1", 10), new KeyValuePair<string, int>("key2", 20) };
        var tuples = new[] { ("key1", 10), ("key2", 20) };

        // Act
        var actual = tuples.ToKeyValuePairs();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion
}
