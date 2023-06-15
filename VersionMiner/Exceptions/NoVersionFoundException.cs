// <copyright file="NoVersionFoundException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.Serialization;
using System.Security;

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if a version value has not been found.
/// </summary>
[Serializable]
public sealed class NoVersionFoundException : Exception
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

    /// <summary>
    /// Initializes a new instance of the <see cref="NoVersionFoundException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private NoVersionFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
