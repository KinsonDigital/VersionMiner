// <copyright file="RepoFileDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Net;
using VersionMiner.Exceptions;

namespace VersionMiner.Services;

/// <inheritdoc/>
public class RepoFileDataService : IRepoFileDataService
{
    private const string BaseUrl = "https://api.github.com";
    private const string AuthHeaderName = "Authorization";
    private readonly IHttpClient client;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoFileDataService"/> class.
    /// </summary>
    /// <param name="client">Makes HTTP requests.</param>
    public RepoFileDataService(IHttpClient client)
    {
        this.client = client;
        this.client.BaseUrl = BaseUrl;
    }

    /// <inheritdoc/>
    public string AuthToken { get; set; } = string.Empty;

    /// <inheritdoc/>
    public async Task<string> GetFileData(string repoOwner, string repoName, string branchName, string filePath)
    {
        if (string.IsNullOrEmpty(repoOwner))
        {
            throw new NullOrEmptyStringException($"The param '{nameof(repoOwner)}' cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(repoName))
        {
            throw new NullOrEmptyStringException($"The param '{nameof(repoName)}' cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(branchName))
        {
            throw new NullOrEmptyStringException($"The param '{nameof(branchName)}' cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(filePath))
        {
            throw new NullOrEmptyStringException($"The param '{nameof(filePath)}' cannot be null or empty.");
        }

        HttpResponse? fileDataResponse;
        var queryString = $"?ref={branchName}";
        var requestUri = $@"/repos/{repoOwner}/{repoName}/contents/{filePath}{queryString}";

        this.client.AddRequestHeader(AuthHeaderName, $"token {AuthToken}");
        this.client.AddRequestHeader("Accept", "application/vnd.github.v3.raw");

        fileDataResponse = await this.client.ExecuteGetAsync(requestUri);

        if (fileDataResponse is null)
        {
            throw new HttpRequestException($"The response from '{this.client.BaseUrl}/{requestUri}' returned null.");
        }

        switch (fileDataResponse.StatusCode)
        {
            case HttpStatusCode.NotFound:
                var notFoundMsg =
                    $"Request failed with status code '{(int)HttpStatusCode.NotFound}({HttpStatusCode.NotFound})' for file '{filePath}'.";
                throw new FileNotFoundException(notFoundMsg);
            case HttpStatusCode.OK:
                return fileDataResponse.Content ?? string.Empty;
            default:
                var errorMsg =
                    $"Request failed with status code '{(int)fileDataResponse.StatusCode}({fileDataResponse.StatusCode})'.";
                errorMsg += $"{Environment.NewLine}{fileDataResponse.ErrorException?.Message}";
                throw new HttpRequestException(errorMsg);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">True to dispose of managed resources.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.client.Dispose();
        }

        this.isDisposed = true;
    }
}
