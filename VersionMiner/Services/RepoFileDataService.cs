// <copyright file="RepoFileDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Net;
using Octokit;
using VersionMiner.Exceptions;
using VersionMiner.Guards;

namespace VersionMiner.Services;

/// <inheritdoc/>
public class RepoFileDataService : IRepoFileDataService
{
    private readonly IRepositoryContentsClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepoFileDataService"/> class.
    /// </summary>
    /// <param name="client">Makes HTTP requests.</param>
    public RepoFileDataService(IRepositoryContentsClient client)
    {
        EnsureThat.CtorParamIsNotNull(client);

        this.client = client;
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

        try
        {
            var result = await this.client.GetAllContentsByRef(repoOwner, repoName, filePath, branchName);

            if (result.Count <= 0)
            {
                throw new NotFoundException(string.Empty, HttpStatusCode.NotFound);
            }

            return result[0].Content;
        }
        catch (NotFoundException)
        {
            var message = $"The file '{filePath}' in the repository '{repoName}' for the owner '{repoOwner}' was not found.";

            throw new NotFoundException(message, HttpStatusCode.NotFound);
        }
    }
}
