// <copyright file="ExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using FluentAssertions;

namespace VersionMinerIntegrationTests;

using FluentAssertions.Execution;

/// <summary>
/// Provides extension methods for the unit test project.
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// Sets the given <paramref name="propValue"/> to the given <paramref name="obj"/> property that
    /// matches the given <paramref name="propName"/>.
    /// </summary>
    /// <param name="obj">The object that contains the property to set.</param>
    /// <param name="propName">The name of the property to look for on the object.</param>
    /// <param name="propValue">The value to set the property value.</param>
    /// <typeparam name="TObj">The type of object.</typeparam>
    /// <typeparam name="TPropValue">The type of property value.</typeparam>
    /// <exception cref="AssertActualExpectedException">
    ///     Thrown if the following occurs:
    /// <list type="bullet">
    ///     <item>The <paramref name="obj"/> is null.</item>
    ///     <item>The <paramref name="propName"/> is null or empty.</item>
    ///     <item>
    ///         A property with the name <paramref name="propName"/> could not be found in the given <paramref name="obj"/>
    ///         or if the found property type does not match the type of the given <paramref name="propValue"/>.
    ///     </item>
    /// </list>
    /// </exception>
    public static void SetPropertyValue<TObj, TPropValue>(this TObj? obj, string? propName, TPropValue propValue)
        where TObj : class
    {
        if (obj is null)
        {
            var exMsg = "Expected: Not Null";
            exMsg += "\nActual: Is Null";
            exMsg += $"\nThe '{nameof(obj)}' parameter must not be null for this assertion to be completed.";
            throw new AssertionFailedException(exMsg);
        }

        if (string.IsNullOrEmpty(propName))
        {
            var exMsg = $"Expected: {(propName is null ? "Not Null" : "Not Empty")}";
            exMsg += $"Actual: {(propName is null ? "Is Null" : "Is Empty")}";
            exMsg += $"pnThe '{nameof(propName)}' parameter must not be null or empty for this assertion to be completed.";

            throw new AssertionFailedException(exMsg);
        }

        var foundProp = obj.GetType().Properties().FirstOrDefault(p => p.Name == propName && p.PropertyType == propValue.GetType());

        if (foundProp is null)
        {
            var exMsg = "Expected: Property to be found";
            exMsg += "\nActual: Property was not found";
            exMsg += $"\nThe property '{propName}' on object '{obj.GetType().Name}' was not found or the property";
            exMsg += " type does not match the parameter '{{nameof(propValue)}}' type.";

            throw new AssertionFailedException(exMsg);
        }

        foundProp.SetValue(obj, propValue);
    }
}
