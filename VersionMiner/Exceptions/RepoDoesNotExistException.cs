// <copyright file="RepoDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.Serialization;
using System.Security;

namespace VersionMiner.Exceptions;

/// <summary>
/// Occurs when a repository does not exist.
/// </summary>
[Serializable]
public sealed class RepoDoesNotExistException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepoDoesNotExistException"/> class.
    /// </summary>
    public RepoDoesNotExistException()
        : base("The repository does not exist.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RepoDoesNotExistException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoDoesNotExistException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public RepoDoesNotExistException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoDoesNotExistException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private RepoDoesNotExistException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
