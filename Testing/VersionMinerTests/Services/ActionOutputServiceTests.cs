// <copyright file="ActionOutputServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Moq;
using VersionMiner.Exceptions;
using VersionMiner.Services;
using VersionMinerTests.Helpers;

namespace VersionMinerTests.Services;

public class ActionOutputServiceTests
{
    private readonly Mock<IGitHubConsoleService> mockConsoleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionOutputServiceTests"/> class.
    /// </summary>
    public ActionOutputServiceTests() => this.mockConsoleService = new Mock<IGitHubConsoleService>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGitHubConsoleServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new ActionOutputService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'gitHubConsoleService')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void SetOutputValue_WhenInvoked_SetsOutputValue()
    {
        // Arrange
        var service = CreateService();

        // Act
        service.SetOutputValue("my-output", "my-value");

        // Assert
        this.mockConsoleService.VerifyOnce(m => m.WriteLine("::set-output name=my-output::my-value"));
    }

    [Fact]
    public void SetOutputValue_WithNullOrEmptyOutputName_ThrowsException()
    {
        // Arrange
        var service = CreateService();

        // Act
        var act = () => service.SetOutputValue(null, It.IsAny<string>());

        // Assert
        act.Should()
            .Throw<NullOrEmptyStringException>()
            .WithMessage("The parameter 'name' must not be null or empty.");
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ActionOutputService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ActionOutputService CreateService() => new ActionOutputService(this.mockConsoleService.Object);
}
