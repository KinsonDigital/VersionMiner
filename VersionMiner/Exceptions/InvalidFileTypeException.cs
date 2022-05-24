// <copyright file="InvalidFileTypeException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if the file type is invalid.
/// </summary>
public class InvalidFileTypeException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFileTypeException"/> class.
    /// </summary>
    public InvalidFileTypeException()
        : base("The file type is invalid.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFileTypeException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidFileTypeException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFileTypeException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public InvalidFileTypeException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
