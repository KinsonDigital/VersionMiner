// <copyright file="CSharpProjFileServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using Moq;
using VersionMiner.Services;

namespace VersionMinerTests.Services;

/// <summary>
/// Tests the <see cref="CSharpProjFileService"/> class.
/// </summary>
public class CSharpProjFileServiceTests
{
    private readonly Mock<IDataParserService> _mockXMLParserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpProjFileServiceTests"/> class.
    /// </summary>
    public CSharpProjFileServiceTests() => _mockXMLParserService = new Mock<IDataParserService>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullXMLParserServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new CSharpProjFileService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'xmlParserService')");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(true, "1.2.3")]
    [InlineData(false, "bad-value")]
    public void GetVersion_WhenInvoked_ReturnsCorrectResult(
        bool keyExists,
        string keyValue)
    {
        // Arrange
        var expectedValue = keyExists ? keyValue : string.Empty;
        _mockXMLParserService.Setup(m
                => m.KeyExists("sample-data", "Version", true))
                    .Returns(keyExists);
        _mockXMLParserService.Setup(m
                => m.GetKeyValue("sample-data", "Version", true))
            .Returns(keyValue);

        var service = CreateService();

        // Act
        var actual = service.GetVersion("sample-data");

        // Assert
        actual.exists.Should().Be(keyExists);
        actual.version.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(true, "1.2.3")]
    [InlineData(false, "bad-value")]
    public void GetFileVersion_WhenInvoked_ReturnsCorrectResult(
        bool keyExists,
        string keyValue)
    {
        // Arrange
        var expectedValue = keyExists ? keyValue : string.Empty;
        _mockXMLParserService.Setup(m
                => m.KeyExists("sample-data", "FileVersion", true))
            .Returns(keyExists);
        _mockXMLParserService.Setup(m
                => m.GetKeyValue("sample-data", "FileVersion", true))
            .Returns(keyValue);

        var service = CreateService();

        // Act
        var actual = service.GetFileVersion("sample-data");

        // Assert
        actual.exists.Should().Be(keyExists);
        actual.version.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(true, "1.2.3")]
    [InlineData(false, "bad-value")]
    public void GetAssemblyVersion_WhenInvoked_ReturnsCorrectResult(
        bool keyExists,
        string keyValue)
    {
        // Arrange
        var expectedValue = keyExists ? keyValue : string.Empty;
        _mockXMLParserService.Setup(m
                => m.KeyExists("sample-data", "AssemblyVersion", true))
            .Returns(keyExists);
        _mockXMLParserService.Setup(m
                => m.GetKeyValue("sample-data", "AssemblyVersion", true))
            .Returns(keyValue);

        var service = CreateService();

        // Act
        var actual = service.GetAssemblyVersion("sample-data");

        // Assert
        actual.exists.Should().Be(keyExists);
        actual.version.Should().Be(expectedValue);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="CSharpProjFileService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private CSharpProjFileService CreateService() => new (_mockXMLParserService.Object);
}
