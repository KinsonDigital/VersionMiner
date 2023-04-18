// <copyright file="App.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace VersionMiner;

/// <summary>
/// Provides simple application information.
/// </summary>
[ExcludeFromCodeCoverage]
public static class App
{
    /// <summary>
    /// Gets the version of the application.
    /// </summary>
    /// <returns>The version of the application.</returns>
    /// <exception cref="ApplicationException">
    ///     Occurs if the entry assembly or assembly data could not be returned.
    /// </exception>
    /// <returns>
    ///     This is the <c>Version</c> XML element in the C# project file.
    /// </returns>
    public static string GetVersion()
    {
        var assembly = Assembly.GetEntryAssembly();

        if (assembly is null)
        {
            throw new ApplicationException("An entry assembly could not be returned to get the application version.");
        }

        var assemblyInfo = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        if (assemblyInfo is null)
        {
            throw new ApplicationException("No assembly information could be found to get the application version.");
        }

        return assemblyInfo.InformationalVersion;
    }
}
