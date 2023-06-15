// <copyright file="NullOrEmptyStringException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.Serialization;
using System.Security;

namespace VersionMiner.Exceptions;

/// <summary>
/// Occurs when a string is null or empty.
/// </summary>
[Serializable]
public sealed class NullOrEmptyStringException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyStringException"/> class.
    /// </summary>
    public NullOrEmptyStringException()
        : base("The string must not be null or empty.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyStringException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NullOrEmptyStringException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyStringException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public NullOrEmptyStringException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NullOrEmptyStringException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private NullOrEmptyStringException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
