// <copyright file="BranchDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Exceptions;

/// <summary>
/// Occurs when a repository branch does not exist.
/// </summary>
public class BranchDoesNotExistException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BranchDoesNotExistException"/> class.
    /// </summary>
    public BranchDoesNotExistException()
        : base("The repository branch does not exist.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BranchDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BranchDoesNotExistException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BranchDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public BranchDoesNotExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
