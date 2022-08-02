// <copyright file="InvalidFileFormatException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if the file format is invalid.
/// </summary>
public class InvalidFileFormatException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFileFormatException"/> class.
    /// </summary>
    public InvalidFileFormatException()
        : base("The file format is invalid.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFileFormatException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidFileFormatException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidFileFormatException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public InvalidFileFormatException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
