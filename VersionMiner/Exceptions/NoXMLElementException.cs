// <copyright file="NoXMLElementException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Runtime.Serialization;
using System.Security;

namespace VersionMiner.Exceptions;

/// <summary>
/// Thrown if an XML element does not contain a value.
/// </summary>
[Serializable]
public sealed class NoXMLElementException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoXMLElementException"/> class.
    /// </summary>
    public NoXMLElementException()
        : base("The XML element does not exist.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoXMLElementException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoXMLElementException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoXMLElementException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public NoXMLElementException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoXMLElementException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private NoXMLElementException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
