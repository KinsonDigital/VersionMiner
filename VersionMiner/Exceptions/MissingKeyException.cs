// <copyright file="MissingKeyException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if a key that holds a version value in a file is missing.
/// </summary>
public class MissingKeyException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
    /// </summary>
    public MissingKeyException()
        : base("The key is missing.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MissingKeyException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MissingKeyException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public MissingKeyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
