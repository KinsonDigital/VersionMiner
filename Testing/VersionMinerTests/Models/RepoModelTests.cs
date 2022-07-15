// <copyright file="RepoModelTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using VersionMiner.Models;
// ReSharper disable UseObjectOrCollectionInitializer
namespace VersionMinerTests.Models;

/// <summary>
/// Tests the <see cref="RepoModel"/> class.
/// </summary>
public class RepoModelTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_HasCorrectDefaultValues()
    {
        // Arrange & Act
        var model = new RepoModel();

        // Assert
        model.Name.Should().BeEmpty();
        model.Owner.Should().BeNull();
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Name_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var model = new RepoModel();

        // Act
        model.Name = "test-name";
        var actual = model.Name;

        // Assert
        actual.Should().Be("test-name");
    }

    [Fact]
    public void Owner_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new OwnerInfoModel() { Login = "test-login", };
        var ownerInfoModel = new OwnerInfoModel() { Login = "test-login", };
        var repoModel = new RepoModel();

        // Act
        repoModel.Owner = ownerInfoModel;
        var actual = repoModel.Owner;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion
}
