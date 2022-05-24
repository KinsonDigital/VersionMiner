// <copyright file="GitHubDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Net;
using RestSharp;
using VersionMiner.Models;

namespace VersionMiner.Services;

// TODO: Need to look into using GITHUB_TOKEN for repo access.  Example: private repos

/// <summary>
/// <inheritdoc/>
/// </summary>
[ExcludeFromCodeCoverage]
public class GitHubDataService : IGitHubDataService
{
    private const string BaseUrl = "https://api.github.com";
    private readonly RestClient _client;
    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubDataService"/> class.
    /// </summary>
    public GitHubDataService() => _client = new RestClient(BaseUrl);

    /// <inheritdoc/>
    public string RepoOwner { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string RepoName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string BranchName { get; set; } = string.Empty;

    /// <inheritdoc/>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>An asynchronous bool result of <b>true</b> if the owner exists.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Occurs if the <see cref="RepoOwner"/> is null or empty.
    /// </exception>
    public async Task<bool> OwnerExists()
    {
        if (string.IsNullOrEmpty(RepoOwner))
        {
            throw new InvalidOperationException($"The '{nameof(RepoOwner)}' value cannot be null or empty.");
        }

        _client.AcceptedContentTypes = new[] { "application/vnd.github.v3+json" };

        const string resourceUrl = "users";
        var fullUrl = $"{BaseUrl}/{resourceUrl}/{RepoOwner}";
        var request = new RestRequest(fullUrl);

        var response = await _client.ExecuteAsync<OwnerInfoModel>(request, Method.Get);

        return response.StatusCode == HttpStatusCode.OK &&
            response.Data is not null &&
            response.Data.Login.ToUpper() == RepoOwner.ToUpper();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>An asynchronous bool result of <b>true</b> if the repository exists.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Occurs if the <see cref="RepoOwner"/> or <see cref="RepoName"/> are <b>null</b> or empty.
    /// </exception>
    public async Task<bool> RepoExists()
    {
        if (string.IsNullOrEmpty(RepoOwner))
        {
            throw new InvalidOperationException($"The '{nameof(RepoOwner)}' value cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(RepoName))
        {
            throw new InvalidOperationException($"The '{nameof(RepoName)}' value cannot be null or empty.");
        }

        _client.AcceptedContentTypes = new[] { "application/vnd.github.v3+json" };

        var fullUrl = $"{BaseUrl}/repos/{RepoOwner}/{RepoName}";
        var request = new RestRequest(fullUrl);

        var response = await _client.ExecuteAsync<RepoModel>(request, Method.Get);

        return response.StatusCode == HttpStatusCode.OK &&
               response.Data is not null &&
               response.Data.Name.ToUpper() == RepoName.ToUpper() &&
               response.Data.Owner.Login.ToUpper() == RepoOwner.ToUpper();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>An asynchronous bool result of <b>true</b> if the repository branch exists.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Occurs if the <see cref="RepoOwner"/>, <see cref="RepoName"/>, or
    ///     <see cref="BranchName"/> are <b>null</b> or empty.
    /// </exception>
    public async Task<bool> BranchExists()
    {
        if (string.IsNullOrEmpty(RepoOwner))
        {
            throw new InvalidOperationException($"The '{nameof(RepoOwner)}' value cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(RepoName))
        {
            throw new InvalidOperationException($"The '{nameof(RepoName)}' value cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(BranchName))
        {
            throw new InvalidOperationException($"The '{nameof(BranchName)}' value cannot be null or empty.");
        }

        _client.AcceptedContentTypes = new[] { "application/vnd.github.v3+json" };

        var fullUrl = $"{BaseUrl}/repos/{RepoOwner}/{RepoName}/branches";
        var request = new RestRequest(fullUrl);

        var response = await _client.ExecuteAsync<BranchModel[]>(request, Method.Get);

        return response.StatusCode == HttpStatusCode.OK &&
               response.Data is not null &&
               response.Data.Any(i => i.Name.ToUpper() == BranchName.ToUpper());
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns>An asynchronous <b>string</b> data result of the file.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if any of the properties are empty or null.
    /// <list type="bullet">
    ///     <item><see cref="RepoOwner"/></item>
    ///     <item><see cref="RepoName"/></item>
    ///     <item><see cref="BranchName"/></item>
    ///     <item><see cref="FilePath"/></item>
    /// </list>
    /// </exception>
    public async Task<string> GetFileData()
    {
        if (string.IsNullOrEmpty(RepoOwner))
        {
            throw new InvalidOperationException($"The '{nameof(RepoOwner)}' value cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(RepoName))
        {
            throw new InvalidOperationException($"The '{nameof(RepoName)}' value cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(BranchName))
        {
            throw new InvalidOperationException($"The '{nameof(BranchName)}' value cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(FilePath))
        {
            throw new InvalidOperationException($"The '{nameof(FilePath)}' value cannot be null or empty.");
        }

        _client.AcceptedContentTypes = new[] { "application/vnd.github.v3.raw" };

        var queryString = $"?ref={BranchName}";
        var repoResourceUrl = $@"/repos/{RepoOwner}/{RepoName}/contents/{FilePath}{queryString}";

        var request = new RestRequest(repoResourceUrl);
        var response = await _client.GetAsync(request);

        switch (response.StatusCode)
        {
            case HttpStatusCode.NotFound:
                throw new FileNotFoundException("The file was not found.", FilePath);
            case HttpStatusCode.OK:
            default:
                return response.Content ?? string.Empty;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_isDisposed)
        {
            return;
        }

        _client.Dispose();
        _isDisposed = true;
    }
}
