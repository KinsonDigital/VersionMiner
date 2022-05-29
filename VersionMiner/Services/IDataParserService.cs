// <copyright file="IDataParserService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Services;

/// <summary>
/// Parses XML data.
/// </summary>
public interface IDataParserService
{
    /// <summary>
    /// Gets the value of a key that matches the given <paramref name="keyName"/>
    /// in the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The structured data.</param>
    /// <param name="keyName">The name of the key that contains a value.</param>
    /// <param name="isKeyCaseSensitive">True if the key name is case sensitive.</param>
    /// <returns>The value of the key.</returns>
    /// <remarks>
    ///     This can be text or more structured data.
    /// </remarks>
    string GetKeyValue(string data, string keyName, bool isKeyCaseSensitive = true);

    /// <summary>
    /// Returns a value indicating whether or not a key with the given <paramref name="keyName"/>
    /// exists in the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The structured data.</param>
    /// <param name="keyName">The name of the key.</param>
    /// <param name="isKeyCaseSensitive">True if the key name is case sensitive.</param>
    /// <returns><c>true</c> if the key exists.</returns>
    bool KeyExists(string data, string keyName, bool isKeyCaseSensitive = true);
}
