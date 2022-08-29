// <copyright file="IHttpClient.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;

namespace VersionMiner.Services;

/// <summary>
/// Provides a class for sending HTTP requests and receiving HTTP responses from a resource identified by a URI.
/// </summary>
public interface IHttpClient : IDisposable
{
    /// <summary>
    /// Gets or sets the BaseUrl property for requests made by this client instance.
    /// </summary>
    string BaseUrl { get; set; }

    /// <summary>
    /// Gets the list of the headers for the next request that will be executed.
    /// </summary>
    ReadOnlyCollection<(string name, string value)> RequestHeaders { get; }

    /// <summary>
    /// Gets the list of headers that are made with every request.
    /// </summary>
    ReadOnlyCollection<(string name, string value)> DefaultHeaders { get; }

    /// <summary>
    /// Adds a header to the next request using the given <paramref name="name"/> and <paramref name="value"/>.
    /// </summary>
    /// <param name="name">The name of the header to add.</param>
    /// <param name="value">The value of the header to add.</param>
    /// <remarks>
    ///     After the next request is made, the headers will be cleared out.
    /// </remarks>
    void AddRequestHeader(string name, string value);

    /// <summary>
    /// Adds the given list of <paramref name="headers"/> to the next request.
    /// </summary>
    /// <param name="headers">The list of headers to add.</param>
    /// <remarks>
    ///     The headers will be cleared out after the next request is made.
    /// </remarks>
    void AddRequestHeaders(IEnumerable<(string name, string value)> headers);

    /// <summary>
    /// Adds a default header using the given <paramref name="name"/> and <paramref name="value"/> to the RestClient.
    /// Used on every request made by this client instance.
    /// </summary>
    /// <param name="name">The name of the header to add.</param>
    /// <param name="value">The value of the header to add.</param>
    void AddDefaultHeader(string name, string value);

    /// <summary>
    /// Adds the given default <paramref name="headers"/> to the RestClient.
    /// Used on every request made by this client instance.
    /// </summary>
    /// <param name="headers">The list of headers to add.</param>
    void AddDefaultHeaders(IEnumerable<(string name, string value)> headers);

    /// <summary>
    /// Removes the first occurence of a header that matches the given header <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The name of the header.</param>
    /// <remarks>
    ///     Multiple headers with the same name are allowed in an HTTP request.
    /// </remarks>
    void RemoveHeader(string name);

    /// <summary>
    /// Clears all of the currently existing headers.
    /// </summary>
    void ClearHeaders();

    /// <summary>
    /// Adds the given <paramref name="contentType"/> to the <c>Accept Header</c>.
    /// </summary>
    /// <param name="contentType">The type of content to add.</param>
    void AddContentType(string contentType);

    /// <summary>
    /// Removes the given <paramref name="contentType"/> to the <c>Accept Header</c>.
    /// </summary>
    /// <param name="contentType">The type of content to remove.</param>
    void RemoveContentType(string contentType);

    /// <summary>
    /// Clears out all of the content types from the <c>Accept Header</c>.
    /// </summary>
    void ClearContentTypes();

    /// <summary>
    /// Executes the request using the GET HTTP method. Exception will be thrown if the request fails.
    /// </summary>
    /// <param name="requestUri">The URI where the request is sent.</param>
    /// <returns>An <see cref="HttpResponse"/> of the request.</returns>
    Task<HttpResponse?> GetAsync(string requestUri);

    /// <summary>
    /// Executes the request using the GET HTTP method. Exception will be thrown if the request fails.
    /// </summary>
    /// <param name="requestUri">The URI where the request is sent.</param>
    /// <typeparam name="T">Target deserialization type.</typeparam>
    /// <returns>
    ///     The deserialized object of type <typeparamref name="T"/>.
    /// </returns>
    Task<T?> GetAsync<T>(string requestUri);

    /// <summary>
    /// Executes an HTTP request asynchronously, authenticating if needed.
    /// </summary>
    /// <param name="requestUri">Request to be executed.</param>
    /// <typeparam name="T">The type of deserialized data.</typeparam>
    /// <returns>
    ///     The asynchronous HTTP response with the deserialized data of type <typeparamref name="T"/>.
    /// </returns>
    Task<DeserializedHttpResponse<T>?> ExecuteGetAsync<T>(string requestUri);

    /// <summary>
    /// Executes an HTTP request asynchronously, authenticating if needed.
    /// </summary>
    /// <param name="requestUri">Request to be executed.</param>
    /// <returns>The asynchronous HTTP response of the HTTP request.</returns>
    Task<HttpResponse?> ExecuteGetAsync(string requestUri);
}
