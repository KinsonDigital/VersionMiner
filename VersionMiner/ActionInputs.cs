// <copyright file="ActionInputs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VersionMiner;

/// <summary>
/// Holds all of the GitHub actions inputs.
/// </summary>
public class ActionInputs
{
    /// <summary>
    /// Gets or sets the owner of the repository.
    /// </summary>
    [Option(
        "repo-owner",
        Required = true,
        HelpText = "Gets or sets the owner of the repository.")]
    public string RepoOwner { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the repository.
    /// </summary>
    [Option(
        "repo-name",
        Required = true,
        HelpText = "Gets or sets the name of the repository.")]
    public string RepoName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the branch where the file lives.
    /// </summary>
    [Option(
        "branch-name",
        Required = true,
        HelpText = "Gets or sets the name of the branch where the file lives.")]
    public string BranchName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path to the file relative to the root of the repository.
    /// </summary>
    [Option(
        "file-path",
        Required = true,
        HelpText = "Gets or sets the path to the file relative to the root of the repository.")]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the type of version we are dealing with.
    /// </summary>
    /// <remarks>
    ///     Examples of possible version types:
    ///     <list type="bullet">
    ///         <item>C# Project File</item>
    ///         <item>NPM JSON File</item>
    ///     </list>
    ///
    /// NOTE: The value is not case sensitive.
    /// </remarks>
    [Option(
        "file-format",
        Required = true,
        HelpText = "The format of the data in the file that contains the version.  Currently the only supported format is 'xml' and is not case sensitive.")]
    public string FileFormat { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of keys that could possibly hold the version in the file.
    /// </summary>
    [Option(
        "version-keys",
        Required = true,
        HelpText = "The key(s) that can possibly hold the version in the file.")]
    public string VersionKeys { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether or not searching for keys will be case sensitive.
    /// </summary>
    [Option(
        "case-sensitive-keys",
        Required = false,
        Default = true,
        HelpText = "If true, then the key searching will be case sensitive.")]
    public bool CaseSensitiveKeys { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the GitHub action should fail if any of the keys
    /// described in the list of <see cref="VersionKeys"/> are missing.
    /// </summary>
    [Option(
        "fail-on-missing-key",
        Required = false,
        Default = true,
        HelpText = "If true, will fail if any of the keys described in the list of 'version-keys' are missing.")]
    public bool FailOnMissingKey { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the GitHub action should fail if all of the key values
    /// described in the list of <see cref="VersionKeys"/> do not match.
    /// </summary>
    [Option(
        "fail-on-key-value-mismatch",
        Required = false,
        Default = false,
        HelpText = "If true, will fail the action if all of the key values listed in the 'version-keys' input do not match.")]
    public bool FailOnKeyValueMismatch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether or not the GitHub action should fail if no version exists.
    /// </summary>
    [Option(
        "fail-when-version-not-found",
        Required = false,
        Default = true,
        HelpText = "If true, the action will fail if the version is not found.")]
    public bool FailWhenVersionNotFound { get; set; } = true;
}
