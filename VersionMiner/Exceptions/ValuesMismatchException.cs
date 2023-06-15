// <copyright file="ValuesMismatchException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.Serialization;
using System.Security;

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if all of the version values from a file do not match.
/// </summary>
[Serializable]
public sealed class ValuesMismatchException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesMismatchException"/> class.
    /// </summary>
    public ValuesMismatchException()
        : base("All values do not match.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesMismatchException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValuesMismatchException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesMismatchException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public ValuesMismatchException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValuesMismatchException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private ValuesMismatchException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
