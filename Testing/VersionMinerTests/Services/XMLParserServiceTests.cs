// <copyright file="XMLParserServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;
using VersionMiner.Exceptions;
using VersionMiner.Services;
using VersionMinerTests.Helpers;

namespace VersionMinerTests.Services;

/// <summary>
/// Tests the <see cref="XMLParserService"/> class.
/// </summary>
public class XMLParserServiceTests
{
    private const string XMLTestDataFileName = "SampleXMLData.xml";

    #region Method Tests
    [Fact]
    public void GetKeyValue_WhenElementDoesNotExist_ThrowsException()
    {
        // Arrange
        var parser = new XMLParserService();
        var sampleXmlData = TestDataLoader.LoadFileData(XMLTestDataFileName);
        const string elementName = "does-not-exist-element";

        // Act
        Action act = () => parser.GetKeyValue(sampleXmlData, elementName);

        // Assert
        act.Should()
            .Throw<NoXMLElementException>()
            .WithMessage($"The XML element '{elementName}' does not exist.");
    }

    [Fact]
    public void GetKeyValue_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var parser = new XMLParserService();
        var sampleXmlData = TestDataLoader.LoadFileData(XMLTestDataFileName);

        // Act
        var actual = parser.GetKeyValue(sampleXmlData, "Item2");

        // Assert
        actual.Should().Be("Item2Value");
    }

    [Theory]
    [InlineData("Item4", true, true)]
    [InlineData("item4", false, true)]
    [InlineData("item4", true, false)]
    [InlineData("not-exist-element", true, false)]
    [InlineData("not-exist-element", false, false)]
    public void KeyExists_WhenInvoked_ReturnsCorrectResult(string elementName, bool isCaseSensitive, bool expected)
    {
        // Arrange
        var parser = new XMLParserService();
        var sampleXmlData = TestDataLoader.LoadFileData(XMLTestDataFileName);

        // Act
        var actual = parser.KeyExists(sampleXmlData, elementName, isCaseSensitive);

        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}
