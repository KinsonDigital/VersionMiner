// <copyright file="HttpResponse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace VersionMiner;

/// <summary>
/// An HTTP request response.
/// </summary>
public class HttpResponse
{
    // ReSharper disable PropertyCanBeMadeInitOnly.Global

    /// <summary>
    /// Gets or sets the content result of an HTTP request.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the HTTP response status code.
    /// </summary>
    public HttpStatusCode StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the exception thrown during the request.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "Not showing code coverage but tested anyways.")]
    public Exception? ErrorException { get; set; }

    // ReSharper restore PropertyCanBeMadeInitOnly.Global
}
