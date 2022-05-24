// <copyright file="IGitHubDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Services;

/// <summary>
/// Provides access to various kinds of GitHub data.
/// </summary>
public interface IGitHubDataService : IDisposable
{
    /// <summary>
    /// Gets or sets the owner of the repository.
    /// </summary>
    public string RepoOwner { get; set; }

    /// <summary>
    /// Gets or sets the name of the repository.
    /// </summary>
    public string RepoName { get; set; }

    /// <summary>
    /// Gets or sets the name of the repository branch.
    /// </summary>
    public string BranchName { get; set; }

    /// <summary>
    /// Gets or sets the path to the file relative to the root of the repository.
    /// </summary>
    public string FilePath { get; set; }

    /// <summary>
    /// Returns a value indicating whether or not the owner of a
    /// repository with the name <see cref="RepoName"/> exists.
    /// </summary>
    /// <returns>An asynchronous bool result of <b>true</b> if the owner exists.</returns>
    Task<bool> OwnerExists();

    /// <summary>
    /// Returns a value indicating whether or not the repository exists.
    /// </summary>
    /// <returns>An asynchronous bool result of <b>true</b> if the repository exists.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Occurs if the <see cref="RepoOwner"/> or <see cref="RepoName"/> are <b>null</b> or empty.
    /// </exception>
    Task<bool> RepoExists();

    /// <summary>
    /// Returns a value indicating whether or not the repository branch exists.
    /// </summary>
    /// <returns>An asynchronous bool result of <b>true</b> if the repository branch exists.</returns>
    Task<bool> BranchExists();

    /// <summary>
    /// Gets the file data.
    /// </summary>
    /// <returns>An asynchronous <b>string</b> data result of the file.</returns>
    Task<string> GetFileData();
}
