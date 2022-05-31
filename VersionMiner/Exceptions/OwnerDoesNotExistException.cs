// <copyright file="OwnerDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Occurs when a user does not exist.
/// </summary>
public class OwnerDoesNotExistException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OwnerDoesNotExistException"/> class.
    /// </summary>
    public OwnerDoesNotExistException()
        : base("The repository owner does not exist.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OwnerDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public OwnerDoesNotExistException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OwnerDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public OwnerDoesNotExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
