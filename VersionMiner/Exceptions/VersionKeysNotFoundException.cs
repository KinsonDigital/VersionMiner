// <copyright file="VersionKeysNotFoundException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if no version keys were supplied or could not be parsed.
/// </summary>
public class VersionKeysNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VersionKeysNotFoundException"/> class.
    /// </summary>
    public VersionKeysNotFoundException()
        : base("No version keys found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionKeysNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public VersionKeysNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionKeysNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public VersionKeysNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
