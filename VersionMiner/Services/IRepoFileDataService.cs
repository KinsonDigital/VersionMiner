// <copyright file="IRepoFileDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner.Services;

/// <summary>
/// Provides the ability to get file data from a repository.
/// </summary>
public interface IRepoFileDataService
{
    /// <summary>
    /// Gets or sets the authorization token to use when making requests for GitHub data.
    /// </summary>
    // ReSharper disable UnusedMemberInSuper.Global
    string AuthToken { get; set; }

    // ReSharper restore UnusedMemberInSuper.Global

    /// <summary>
    /// Returns the data from a file in a repository that matches the given <paramref name="repoName"/>,
    /// in a GIT branch that matches the given <paramref name="branchName"/> at the given <paramref name="filePath"/>
    /// for a repository owner that matches the given <paramref name="repoOwner"/>.
    /// </summary>
    /// <param name="repoOwner">The owner of a repository.</param>
    /// <param name="repoName">The name of a repository.</param>
    /// <param name="branchName">The name of a branch.</param>
    /// <param name="filePath">The path to the file relative to the root of the repository.</param>
    /// <returns>The file data.</returns>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="repoOwner"/> and <paramref name="repoName"/> are not case sensitive.
    ///     </para>
    ///     <para>
    ///         The <paramref name="branchName"/> is case sensitive.
    ///     </para>
    /// </remarks>
    Task<string> GetFileData(string repoOwner, string repoName, string branchName, string filePath);
}
