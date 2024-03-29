﻿// <copyright file="ExtensionMethodsForTesting.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Reflection;
using CommandLine;

namespace VersionMinerTests.Helpers;

using FluentAssertions.Execution;

/// <summary>
/// Provides extension/helper methods to assist in unit testing.
/// </summary>
public static class ExtensionMethodsForTesting
{
    /// <summary>
    /// Gets an attribute of type <typeparamref name="T"/> on a property of the object
    /// that matches with the name <paramref name="propName"/>.
    /// </summary>
    /// <param name="value">The object that contains the property.</param>
    /// <param name="propName">The name of the property on the object.</param>
    /// <typeparam name="T">The type of attribute on the property.</typeparam>
    /// <returns>The attribute if it exists.</returns>
    /// <exception cref="AssertionFailedException">
    ///     Thrown if the property or attribute does not exist.
    /// </exception>
    public static T GetAttrFromProp<T>(this object value, string propName)
        where T : Attribute
    {
        var props = value.GetType().GetProperties();
        var noPropsAssertMsg = string.IsNullOrEmpty(propName)
            ? $"Cannot get an attribute on a property when the '{nameof(propName)}' parameter is null or empty."
            : $"Cannot get an attribute on a property when no property with the name '{propName}' exists.";
        var noPropsExMsg = "Expected: at least 1 item";
        noPropsExMsg += "\nActual: 0 items";
        noPropsExMsg += $"\n{noPropsAssertMsg}";

        if (props.Length <= 0)
        {
            throw new AssertionFailedException(noPropsExMsg);
        }

        var propNotFoundAssertMsg = $"Cannot get an attribute on the property '{propName}' if the property does not exist.";
        var foundProp = (from p in props
            where p.Name == propName
            select p).FirstOrDefault();

        if (foundProp is null)
        {
            var notFoundPropExMsg = "Expected: not to be null";
            notFoundPropExMsg += "\nActual: was null";
            notFoundPropExMsg += $"\n{propNotFoundAssertMsg}";

            throw new AssertionFailedException(notFoundPropExMsg);
        }

        var noAttrsAssertMsg = $"Cannot get an attribute when the property '{propName}' does not have any attributes.";
        var attrs = foundProp.GetCustomAttributes<T>().ToArray();

        if (attrs.Length <= 0)
        {
            var noAttrsExMsg = "Expected: at least 1 item";
            noAttrsExMsg += "\nActual: 0 items";
            noAttrsExMsg += $"\n{noAttrsAssertMsg}";

            throw new AssertionFailedException(noAttrsExMsg);
        }

        return attrs[0];
    }

    /// <summary>
    /// Asserts the properties below.
    /// <list type="bullet">
    ///     <item><see cref="OptionAttribute"/>.<see cref="OptionAttribute.LongName"/></item>
    ///     <item><see cref="OptionAttribute"/>.<see cref="BaseAttribute.Required"/></item>
    ///     <item><see cref="OptionAttribute"/>.<see cref="BaseAttribute.Default"/></item>
    ///     <item><see cref="OptionAttribute"/>.<see cref="BaseAttribute.HelpText"/></item>
    /// </list>
    /// </summary>
    /// <param name="value">The attribute to assert.</param>
    /// <param name="longNameExpected">The expected value of the <see cref="OptionAttribute.LongName"/> property.</param>
    /// <param name="requiredExpected">The expected value of the <see cref="OptionAttribute.Required"/> property.</param>
    /// <param name="defaultExpected">The expected value of the <see cref="BaseAttribute.Default"/> property.</param>
    /// <param name="helpTextExpected">The expected value of the <see cref="OptionAttribute.HelpText"/> property.</param>
    /// <exception cref="AssertionFailedException">
    ///     Thrown if the any of the properties are not the correct values.
    /// </exception>
    public static void AssertOptionAttrProps(this OptionAttribute value,
        string longNameExpected,
        bool requiredExpected,
        object defaultExpected,
        string helpTextExpected)
    {
        if (value.LongName != longNameExpected)
        {
            throw new AssertionFailedException(
                $"Expected: {longNameExpected}" +
                "\nActual: value.LongName" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.LongName)}' property value is not correct for option '{longNameExpected}'.");
        }

        if (value.Required != requiredExpected)
        {
            throw new AssertionFailedException(
                $"{requiredExpected}" +
                $"\n{value.Required}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.Required)}' property value is not correct for option '{longNameExpected}'.");
        }

        if (value.Default.ToString() != defaultExpected.ToString())
        {
            throw new AssertionFailedException(
                $"{defaultExpected}" +
                $"\n{value.Default}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.Default)}' property value is not correct for option '{defaultExpected}'.");
        }

        if (value.HelpText != helpTextExpected)
        {
            throw new AssertionFailedException(
                $"{helpTextExpected}" +
                $"\n{value.HelpText}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.HelpText)}' property value is not correct for option '{longNameExpected}'.");
        }
    }

    /// <summary>
    /// Asserts the properties below:
    /// <list type="bullet">
    ///     <item><see cref="OptionAttribute"/>.<see cref="OptionAttribute.LongName"/></item>
    ///     <item><see cref="OptionAttribute"/>.<see cref="BaseAttribute.Required"/></item>
    ///     <item><see cref="OptionAttribute"/>.<see cref="BaseAttribute.HelpText"/></item>
    /// </list>
    /// </summary>
    /// <param name="value">The attribute to assert.</param>
    /// <param name="longNameExpected">The expected value of the <see cref="OptionAttribute.LongName"/> property.</param>
    /// <param name="requiredExpected">The expected value of the <see cref="OptionAttribute.Required"/> property.</param>
    /// <param name="helpTextExpected">The expected value of the <see cref="OptionAttribute.HelpText"/> property.</param>
    /// <exception cref="AssertionFailedException">
    ///     Thrown if any of the properties are not the correct values.
    /// </exception>
    public static void AssertOptionAttrProps(this OptionAttribute value,
        string longNameExpected,
        bool requiredExpected,
        string helpTextExpected)
    {
        if (value.LongName != longNameExpected)
        {
            throw new AssertionFailedException(
                $"{longNameExpected}" +
                $"\n{value.LongName}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.LongName)}' property value is not correct for option '{longNameExpected}'.");
        }

        if (value.Required != requiredExpected)
        {
            throw new AssertionFailedException(
                $"{requiredExpected}" +
                $"\n{value.Required}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.Required)}' property value is not correct for option '{longNameExpected}'.");
        }

        if (value.HelpText != helpTextExpected)
        {
            throw new AssertionFailedException(
                $"{helpTextExpected}" +
                $"\n{value.HelpText}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.HelpText)}' property value is not correct for option '{longNameExpected}'.");
        }
    }

    /// <summary>
    /// Asserts the properties below:
    /// <list type="bullet">
    ///     <item><see cref="OptionAttribute"/>.<see cref="OptionAttribute.LongName"/></item>
    ///     <item><see cref="OptionAttribute"/>.<see cref="BaseAttribute.Required"/></item>
    ///     <item><see cref="OptionAttribute"/>.<see cref="BaseAttribute.HelpText"/></item>
    /// </list>
    /// </summary>
    /// <param name="value">The attribute to assert.</param>
    /// <param name="longNameExpected">The expected value of the <see cref="OptionAttribute.LongName"/> property.</param>
    /// <param name="helpTextExpected">The expected value of the <see cref="OptionAttribute.HelpText"/> property.</param>
    /// <exception cref="AssertionFailedException">
    ///     Thrown if the any of the properties are not the correct values.
    /// </exception>
    public static void AssertOptionAttrProps(this OptionAttribute value,
        string longNameExpected,
        string helpTextExpected)
    {
        if (value.LongName != longNameExpected)
        {
            throw new AssertionFailedException(
                $"{longNameExpected}" +
                $"\n{value.LongName}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.LongName)}' property value is not correct.");
        }

        if (value.HelpText != helpTextExpected)
        {
            throw new AssertionFailedException(
                $"{helpTextExpected}" +
                $"\n{value.HelpText}" +
                $"The '{nameof(OptionAttribute)}.{nameof(OptionAttribute.HelpText)}' property value is not correct.");
        }
    }

    /// <summary>
    /// Sets the value of a property that matches the given <paramref name="propName"/> with the given
    /// <paramref name="propValue"/> for the given <paramref name="obj"/>.
    /// </summary>
    /// <param name="obj">The object that contains the property to set.</param>
    /// <param name="propName">The name of the property.</param>
    /// <param name="propValue">The value of the property.</param>
    /// <typeparam name="TObj">The type of object.</typeparam>
    /// <typeparam name="TPropValue">The type of property value.</typeparam>
    /// <exception cref="AssertionFailedException">
    ///     Occurs if the parameters <paramref name="obj"/> or <paramref name="propName"/> are null.
    ///     Occurs if the property was not found.
    /// </exception>
    public static void SetPropValue<TObj, TPropValue>(this TObj obj, string propName, TPropValue propValue)
    {
        if (obj is null)
        {
            throw new AssertionFailedException(
                "Expected: Not to be null." +
                "\nActual: Is null." +
                $"The parameter '{nameof(propName)}' must not be null to set the property value");
        }

        if (string.IsNullOrEmpty(propName))
        {
            throw new AssertionFailedException(
                "Expected: Not to be null or empty." +
                "\nActual: Is null or empty." +
                $"The parameter '{nameof(propName)}' must not be null or empty to set the property value");
        }

        var foundProp = (from p in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
            where p.Name == propName
            select p).First();

        if (foundProp is null)
        {
            throw new AssertionFailedException(
                "Expected: To exist." +
                "\nActual: Does not exit." +
                $"A property with the name '{propName}' does not exit in the given '{nameof(obj)}' parameter");
        }

        try
        {
            foundProp.SetValue(obj, propValue);
        }
        catch (Exception e)
        {
            throw new AssertionFailedException(
                $"Expected: Property '{propName}' value to be set." +
                $"\nActual: Property '{propName}' value was not set." +
                $"\nSomething went wrong with setting the property '{propName}' value in the '{obj}' parameter.\n{e.Message}");
        }
    }
}
