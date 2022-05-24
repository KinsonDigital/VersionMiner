// <copyright file="ExtensionMethodTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

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
    #endregion
}
