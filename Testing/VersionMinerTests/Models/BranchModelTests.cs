// <copyright file="BranchModelTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using VersionMiner.Models;
// ReSharper disable UseObjectOrCollectionInitializer
namespace VersionMinerTests.Models;

/// <summary>
/// Tests the <see cref="BranchModel"/> class.
/// </summary>
public class BranchModelTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_HasCorrectDefaultValues()
    {
        // Arrange & Act
        var model = new BranchModel();

        // Assert
        model.Name.Should().BeEmpty();
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Name_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var model = new BranchModel();

        // Act
        model.Name = "test-name";
        var actual = model.Name;

        // Assert
        actual.Should().Be("test-name");
    }
    #endregion
}
