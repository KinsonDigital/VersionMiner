// <copyright file="XMLParserService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Text;
using System.Xml.Linq;
using VersionMiner.Exceptions;

namespace VersionMiner.Services;

/// <summary>
/// Parses XML data.
/// </summary>
public class XMLParserService : IDataParserService
{
    /// <inheritdoc />
    public string GetKeyValue(string data, string keyName, bool isKeyCaseSensitive = true)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var element = XElement.Load(stream);

        var elements = element.Descendants().ToArray();

        if (KeyExists(data, keyName, isKeyCaseSensitive) is false)
        {
            throw new NoXMLElementException($"The XML element '{keyName}' does not exist.");
        }

        var keyValue = elements
            .Where(e => isKeyCaseSensitive
                ? e.Name == keyName
                : e.Name.ToString().Equals(keyName, StringComparison.OrdinalIgnoreCase))
            .Select(e => e.Value).First();

        return keyValue;
    }

    /// <inheritdoc />
    public bool KeyExists(string data, string keyName, bool isKeyCaseSensitive = true)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));
        var element = XElement.Load(stream);

        var foundElement = element.Descendants().FirstOrDefault(e =>
            isKeyCaseSensitive
                ? e.Name == keyName
                : e.Name.ToString().Equals(keyName, StringComparison.OrdinalIgnoreCase));

        return foundElement != null;
    }
}
