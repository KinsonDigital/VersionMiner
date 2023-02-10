// <copyright file="IEnvVarService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Services;

/// <summary>
/// Provides the ability to deal with environment variables in the operating system.
/// </summary>
public interface IEnvVarService
{
    /// <summary>
    /// Retrieves the value of an environment variable from the current process.
    /// </summary>
    /// <param name="name">The name of the environment variable.</param>
    /// <param name="target">One of the <see cref="EnvironmentVariableTarget"/> values. Only <see cref="EnvironmentVariableTarget.Process"/>
    /// is supported on .NET running on Unix-based systems.</param>
    /// <returns>
    ///     The value of the environment variable specified by variable, or empty if the environment variable is not found.
    /// </returns>
    string GetEnvironmentVariable(string name, EnvironmentVariableTarget target = EnvironmentVariableTarget.Process);
}
