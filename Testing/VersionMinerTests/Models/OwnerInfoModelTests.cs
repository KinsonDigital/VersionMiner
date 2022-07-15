// <copyright file="OwnerInfoModelTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using VersionMiner.Models;
// ReSharper disable UseObjectOrCollectionInitializer
namespace VersionMinerTests.Models;

/// <summary>
/// Tests the <see cref="OwnerInfoModel"/> class.
/// </summary>
public class OwnerInfoModelTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_HasCorrectDefaultValues()
    {
        // Arrange & Act
        var model = new OwnerInfoModel();

        // Assert
        model.Login.Should().BeEmpty();
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Login_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var model = new OwnerInfoModel();

        // Act
        model.Login = "test-login";
        var actual = model.Login;

        // Assert
        actual.Should().Be("test-login");
    }
    #endregion
}
