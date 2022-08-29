// <copyright file="HttpResponseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using VersionMiner;

// ReSharper disable UseObjectOrCollectionInitializer
namespace VersionMinerTests;

/// <summary>
/// Tests the <see cref="HttpResponse"/> class.
/// </summary>
public class HttpResponseTests
{
    #region Prop Tests
    [Fact]
    public void Content_WhenGettingDefaultValue_ShouldBeNull()
    {
        // Arrange
        var response = new HttpResponse();

        // Assert
        response.Content.Should().BeNull();
    }

    [Fact]
    public void Content_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var response = new HttpResponse();

        // Act
        response.Content = "test-content";

        // Assert
        response.Content.Should().Be("test-content");
    }

    [Fact]
    public void ErrorException_WhenGettingDefaultValue_ReturnsNull()
    {
        // Arrange
        var response = new HttpResponse();

        // Assert
        response.ErrorException.Should().BeNull();
    }

    [Fact]
    public void ErrorException_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expectedException = new Exception("test-exception");

        var response = new HttpResponse();

        // Act
        response.ErrorException = expectedException;

        // Assert
        response.ErrorException.Should().NotBeNull();
        response.ErrorException.Should().BeSameAs(expectedException);
        response.ErrorException.Message.Should().Be("test-exception");
    }
    #endregion
}
