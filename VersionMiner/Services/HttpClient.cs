// <copyright file="HttpClient.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using RestSharp;
using VersionMiner.Exceptions;

namespace VersionMiner.Services;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
public sealed class HttpClient : IHttpClient
{
    private readonly RestClient restClient;
    private readonly List<(string name, string value)> nextRequestHeaders = new ();
    private string baseUrl = string.Empty;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpClient"/> class.
    /// </summary>
    public HttpClient() => this.restClient = new RestClient();

    /// <inheritdoc/>
    public string BaseUrl
    {
        get => this.baseUrl;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new NullOrEmptyStringException($"The property '{this.baseUrl}' value cannot be null or empty.");
            }

            while (value.EndsWith('/'))
            {
                value = value.TrimEnd('/');
            }

            this.baseUrl = value;
        }
    }

    /// <inheritdoc/>
    public ReadOnlyCollection<(string name, string value)> RequestHeaders
        => new ReadOnlyCollection<(string name, string value)>(this.nextRequestHeaders.ToArray());

    /// <inheritdoc/>
    public ReadOnlyCollection<(string name, string value)> DefaultHeaders
    {
        get
        {
            var parameters = this.restClient.DefaultParameters.ToArray();

            var result = new List<(string, string)>();

            foreach (Parameter parameter in parameters)
            {
                if (parameter.Name is null || parameter.Value is null)
                {
                    continue;
                }

                result.Add((parameter.Name, (string)parameter.Value));
            }

            return new ReadOnlyCollection<(string name, string value)>(result.ToArray());
        }
    }

    /// <inheritdoc/>
    /// <exception cref="Exception">
    ///     Invoked when a default header exists.
    /// </exception>
    public void AddRequestHeader(string name, string value)
    {
        if (this.nextRequestHeaders.Any(h => h.name == name && h.value == value))
        {
            return;
        }

        this.nextRequestHeaders.Add((name, value));
    }

    /// <inheritdoc/>
    /// <exception cref="Exception">
    ///     Invoked when a default header exists.
    /// </exception>
    public void AddRequestHeaders(IEnumerable<(string name, string value)> headers)
    {
        foreach ((string name, string value) header in headers)
        {
            AddRequestHeader(header.name, header.value);
        }
    }

    /// <inheritdoc/>
    /// <exception cref="Exception">
    ///     Invoked when a default header exists.
    /// </exception>
    public void AddDefaultHeader(string name, string value) => this.restClient.AddDefaultHeader(name, value);

    /// <inheritdoc/>
    /// <exception cref="Exception">
    ///     Invoked when the default header exists.
    /// </exception>
    public void AddDefaultHeaders(IEnumerable<(string name, string value)> headers)
    {
        var defaultHeaders = new Dictionary<string, string>();

        foreach ((string name, string value) header in headers)
        {
            if (defaultHeaders.ContainsKey(header.name))
            {
                throw new HeaderAlreadyExistsException($"The header '{header.name}' already exists.");
            }

            defaultHeaders.Add(header.name, header.value);
        }

        this.restClient.AddDefaultHeaders(defaultHeaders);
    }

    /// <inheritdoc/>
    public void RemoveHeader(string name)
    {
        var foundHeader = this.nextRequestHeaders.Where(h => h.name == name).ToArray();

        if (foundHeader.Length > 0)
        {
            this.nextRequestHeaders.Remove(foundHeader[0]);
        }
    }

    /// <inheritdoc/>
    public void ClearHeaders() => this.nextRequestHeaders.Clear();

    /// <inheritdoc/>
    public void AddContentType(string contentType)
        => this.restClient.AcceptedContentTypes = new List<string>(this.restClient.AcceptedContentTypes) { contentType }.ToArray();

    /// <inheritdoc/>
    public void RemoveContentType(string contentType)
    {
        var types = new List<string>(this.restClient.AcceptedContentTypes);

        types.Remove(contentType);

        this.restClient.AcceptedContentTypes = types.ToArray();
    }

    /// <inheritdoc/>
    public void ClearContentTypes() => this.restClient.AcceptedContentTypes = Array.Empty<string>();

    /// <inheritdoc/>
    public async Task<HttpResponse?> GetAsync(string requestUri)
    {
        if (string.IsNullOrEmpty(requestUri))
        {
            throw new NullOrEmptyStringException($"The argument '{requestUri}' cannot be null or empty.");
        }

        while (requestUri.StartsWith('/'))
        {
            requestUri = requestUri.TrimStart('/');
        }

        var fullUrl = $"{BaseUrl}/{requestUri}";

        var request = new RestRequest(fullUrl);
        request.AddHeaders(this.nextRequestHeaders.ToKeyValuePairs().ToCollection());
        this.nextRequestHeaders.Clear();

        var response = await this.restClient.GetAsync(request);

        return new HttpResponse
        {
            Content = response.Content,
            ErrorException = response.ErrorException,
            StatusCode = response.StatusCode,
        };
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(string requestUri)
    {
        if (string.IsNullOrEmpty(requestUri))
        {
            throw new NullOrEmptyStringException($"The argument '{requestUri}' cannot be null or empty.");
        }

        while (requestUri.StartsWith('/'))
        {
            requestUri = requestUri.TrimStart('/');
        }

        var fullUrl = $"{BaseUrl}/{requestUri}";

        var request = new RestRequest(fullUrl);
        request.AddHeaders(this.nextRequestHeaders.ToKeyValuePairs().ToCollection());
        this.nextRequestHeaders.Clear();

        var response = await this.restClient.GetAsync<T>(request);

        return response;
    }

    /// <inheritdoc/>
    public async Task<DeserializedHttpResponse<T>?> ExecuteGetAsync<T>(string requestUri)
    {
        while (requestUri.StartsWith('/'))
        {
            requestUri = requestUri.TrimStart('/');
        }

        var fullUrl = $"{BaseUrl}/{requestUri}";
        var request = new RestRequest(fullUrl);
        request.AddHeaders(this.nextRequestHeaders.ToKeyValuePairs().ToCollection());
        this.nextRequestHeaders.Clear();

        var response = await this.restClient.ExecuteGetAsync<T>(request);

        return new DeserializedHttpResponse<T>
        {
            Data = response.Data,
            Content = response.Content,
            StatusCode = response.StatusCode,
            ErrorException = response.ErrorException,
        };
    }

    /// <inheritdoc/>
    public async Task<HttpResponse?> ExecuteGetAsync(string requestUri)
    {
        while (requestUri.StartsWith('/'))
        {
            requestUri = requestUri.TrimStart('/');
        }

        var fullUrl = $"{BaseUrl}/{requestUri}";
        var request = new RestRequest(fullUrl);
        request.AddHeaders(this.nextRequestHeaders.ToKeyValuePairs().ToCollection());
        this.nextRequestHeaders.Clear();

        var response = await this.restClient.ExecuteGetAsync(request);

        return new HttpResponse
        {
            Content = response.Content,
            StatusCode = response.StatusCode,
            ErrorException = response.ErrorException,
        };
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.restClient.Dispose();

        this.isDisposed = true;
    }
}
