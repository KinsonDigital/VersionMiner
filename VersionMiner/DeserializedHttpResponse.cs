// <copyright file="DeserializedHttpResponse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace VersionMiner;

/// <summary>
/// An HTTP response that contains deserialized data.
/// </summary>
/// <typeparam name="T">The type of data that is deserialized.</typeparam>
[ExcludeFromCodeCoverage(Justification = "Not showing code coverage but tested anyways.")]
public class DeserializedHttpResponse<T> : HttpResponse
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializedHttpResponse{T}"/> class.
    /// </summary>
    public DeserializedHttpResponse()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeserializedHttpResponse{T}"/> class.
    /// </summary>
    /// <param name="statusCode">The status code of the HTTP request when the exception occurred.</param>
    public DeserializedHttpResponse(HttpStatusCode statusCode) => StatusCode = statusCode;

    /// <summary>
    /// Gets or sets the deserialized data.
    /// </summary>
    public T? Data { get; set; }
}
