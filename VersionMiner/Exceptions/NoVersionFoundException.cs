// <copyright file="NoVersionFoundException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if a version value has not been found.
/// </summary>
public class NoVersionFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoVersionFoundException"/> class.
    /// </summary>
    public NoVersionFoundException()
        : base("No version value found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoVersionFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoVersionFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoVersionFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public NoVersionFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
