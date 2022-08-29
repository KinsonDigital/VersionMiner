// <copyright file="HeaderAlreadyExistsException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Occurs when an HTTP header exists.
/// </summary>
public class HeaderAlreadyExistsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderAlreadyExistsException"/> class.
    /// </summary>
    public HeaderAlreadyExistsException()
        : base("The header already exists.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public HeaderAlreadyExistsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HeaderAlreadyExistsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public HeaderAlreadyExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
