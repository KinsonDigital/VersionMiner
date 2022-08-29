// <copyright file="DeserializedHttpResponseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Net;
using FluentAssertions;
using VersionMiner;

// ReSharper disable UseObjectOrCollectionInitializer
namespace VersionMinerTests;

/// <summary>
/// Tests the <see cref="DeserializedHttpResponse{T}"/> class.
/// </summary>
public class DeserializedHttpResponseTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvokedWithStatusCode_SetsStatusCodeProp()
    {
        // Arrange
        var response = new DeserializedHttpResponse<string>(HttpStatusCode.Accepted);

        // Act
        var actual = response.StatusCode;

        // Assert
        actual.Should().Be(HttpStatusCode.Accepted);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Data_WhenGettingDefaultValue_ShouldBeNull()
    {
        // Arrange
        var response = new DeserializedHttpResponse<string>();

        // Assert
        response.Data.Should().BeNull();
    }

    [Fact]
    public void Data_WhenSettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var response = new DeserializedHttpResponse<string>();

        // Act
        response.Data = "test-data";

        // Assert
        response.Data.Should().Be("test-data");
    }
    #endregion
}
