// <copyright file="EnsureThatTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using VersionMiner.Guards;

namespace VersionMinerTests.Guards;

/// <summary>
/// Tests the <see cref="EnsureThat"/> class.
/// </summary>
public class EnsureThatTests
{
    #region Method Tests
    [Fact]
    public void CtorParamIsNotNull_WithNullValue_ThrowsException()
    {
        // Arrange
        object? nullObj = null;

        // Act
        var act = () => EnsureThat.CtorParamIsNotNull(nullObj);

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'nullObj')");
    }

    [Fact]
    public void CtorParamIsNotNull_WithNonNullValue_DoesNotThrowException()
    {
        // Arrange
        object nonNullObj = "non-null-obj";

        // Act & Assert
        var act = () => EnsureThat.CtorParamIsNotNull(nonNullObj);

        act.Should().NotThrow<Exception>();
    }
    #endregion
}
