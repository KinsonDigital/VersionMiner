// <copyright file="VersionKeysNotFoundException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.Serialization;
using System.Security;

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if no version keys were supplied or could not be parsed.
/// </summary>
[Serializable]
public sealed class VersionKeysNotFoundException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VersionKeysNotFoundException"/> class.
    /// </summary>
    public VersionKeysNotFoundException()
        : base("No version keys found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionKeysNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public VersionKeysNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionKeysNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public VersionKeysNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VersionKeysNotFoundException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private VersionKeysNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
