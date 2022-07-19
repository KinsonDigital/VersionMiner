// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Text;

namespace VersionMiner;

/// <summary>
/// Provides extensions.
/// </summary>
public static class ExtensionMethods
{
    private static readonly char[] Letters =
    {
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h',
        'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
        'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
        'y', 'z',
    };

    /// <summary>
    /// Returns a value indicating whether or not the given <b>string</b> <paramref name="value"/>
    /// contains a drive letter with other required syntax.
    /// </summary>
    /// <param name="value">The value that might be a path.</param>
    /// <returns><b>true</b> if the path contains a drive.</returns>
    /// <returns>
    ///     A value of <b>true</b> means the path <c>is not</c> a relative path.
    /// </returns>
    public static bool PathContainsDrive(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        if (value.Length < 3)
        {
            return false;
        }

        var startsWithLetter = Letters.Contains(value.ToLower()[0]);
        var secondCharIsColon = value[1] == ':';
        var thirdCharIsSlash = value[2] == '\\' || value[2] == '/';

        return startsWithLetter && secondCharIsColon && thirdCharIsSlash;
    }

    /// <summary>
    /// Removes the given <c>string</c> <paramref name="value"/> from the beginning
    /// of this string.
    /// </summary>
    /// <param name="thisStr">The <c>string</c> value to trim.</param>
    /// <param name="value">The value to trim from the beginning of this <c>string</c>.</param>
    /// <returns>The trimmed <c>string</c>.</returns>
    /// <remarks>
    ///     This is not case sensitive.
    /// </remarks>
    public static string TrimStart(this string thisStr, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return thisStr;
        }

        var valueIndex = thisStr.ToLower().IndexOf(value.ToLower(), StringComparison.Ordinal);

        if (valueIndex != 0)
        {
            return thisStr;
        }

        var result = new StringBuilder();

        for (var i = 0; i < thisStr.Length; i++)
        {
            if (i >= valueIndex && i <= valueIndex + value.Length - 1)
            {
                continue;
            }

            result.Append(thisStr[i]);
        }

        return result.ToString();
    }
}
