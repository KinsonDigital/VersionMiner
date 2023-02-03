// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
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

    /// <summary>
    /// Converts the given list (<typeparamref name="TKey"/>), <typeparamref name="TValue"/>) tuples
    /// to a list of <see cref="IEnumerable{T}"/> items of type <see cref="KeyValuePair{TKey,TValue}"/>.
    /// </summary>
    /// <param name="pairs">The pairs to convert.</param>
    /// <typeparam name="TKey">The type of key.</typeparam>
    /// <typeparam name="TValue">The type of value.</typeparam>
    /// <returns>
    ///     The tuples converted to a list of <see cref="KeyValuePair{TKey,TValue}"/> items.
    /// </returns>
    public static IEnumerable<KeyValuePair<TKey, TValue>> ToKeyValuePairs<TKey, TValue>(this IEnumerable<(TKey key, TValue value)> pairs)
        => pairs.Select(pair => new KeyValuePair<TKey, TValue>(pair.key, pair.value)).ToArray();

    /// <summary>
    /// Converts the given list of <see cref="IEnumerable{T}"/> items to a collection of <see cref="Collection{T}"/> items.
    /// </summary>
    /// <param name="items">The items to convert.</param>
    /// <typeparam name="T">The type of items of the list and resulting collection.</typeparam>
    /// <returns>
    ///     The <see cref="Collection{T}"/> equivalent of the given <paramref name="items"/>.
    /// </returns>
    public static ICollection<T> ToCollection<T>(this IEnumerable<T> items) => new Collection<T>(items.ToList());
}
