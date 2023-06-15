// <copyright file="GitHubAction.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Octokit;
using VersionMiner.Exceptions;
using VersionMiner.Guards;
using VersionMiner.Services;

namespace VersionMiner;

/// <inheritdoc/>
public sealed class GitHubAction : IGitHubAction
{
    private const string VersionOutputName = "version";
    private const string XMLFileFormat = "xml";
    private readonly IGitHubConsoleService consoleService;
    private readonly IGitHubClient githubClient;
    private readonly IRepoFileDataService repoFileDataService;
    private readonly IDataParserService xmlParserService;
    private readonly IActionOutputService actionOutputService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubAction"/> class.
    /// </summary>
    /// <param name="consoleService">Writes to the console.</param>
    /// <param name="githubClient">Gives access to GitHub related data.</param>
    /// <param name="repoFileDataService">Gets file data from a branch in a repository.</param>
    /// <param name="xmlParserService">Parses XML data.</param>
    /// <param name="actionOutputService">Sets the output data of the action.</param>
    public GitHubAction(
        IGitHubConsoleService consoleService,
        IGitHubClient githubClient,
        IRepoFileDataService repoFileDataService,
        IDataParserService xmlParserService,
        IActionOutputService actionOutputService)
    {
        EnsureThat.CtorParamIsNotNull(consoleService);
        EnsureThat.CtorParamIsNotNull(githubClient);
        EnsureThat.CtorParamIsNotNull(repoFileDataService);
        EnsureThat.CtorParamIsNotNull(xmlParserService);
        EnsureThat.CtorParamIsNotNull(actionOutputService);

        this.consoleService = consoleService;
        this.githubClient = githubClient;
        this.repoFileDataService = repoFileDataService;
        this.xmlParserService = xmlParserService;
        this.actionOutputService = actionOutputService;
    }

    /// <inheritdoc/>
    public async Task Run(ActionInputs inputs, Action onCompleted, Action<Exception> onError)
    {
        ShowWelcomeMessage();

        LogIfBranchNeedsTrimming(inputs);

        try
        {
            if (inputs.FileFormat.ToLower() != XMLFileFormat)
            {
                var exMsg = $"The 'file-format' value of '{inputs.FileFormat}' is invalid.";
                exMsg += $"{Environment.NewLine}The only file format currently supported is XML.";
                throw new InvalidFileFormatException(exMsg);
            }

            SetupToken(inputs.RepoToken);

            this.consoleService.Write($"✅️Verifying if the repository '{inputs.RepoName}' exists . . . ");

            var repo = await GetIfRepoExists(inputs.RepoOwner, inputs.RepoName);

            this.consoleService.Write("the repository exists.", true, 2);
            this.consoleService.Write($"✅️Verifying if the repository branch '{inputs.BranchName}' exists . . . ");

            var branch = await this.githubClient.Repository.Branch.Get(repo.Id, inputs.BranchName);

            if (branch is null)
            {
                throw new HttpRequestException($"The repository branch '{inputs.BranchName}' does not exist.");
            }

            this.consoleService.Write("the branch exists.", true, 2);
            this.consoleService.Write($"✅️Getting data for file '{inputs.FilePath}' . . . ");
            this.repoFileDataService.AuthToken = inputs.RepoToken;

            var fileData = await this.repoFileDataService.GetFileData(
                inputs.RepoOwner,
                inputs.RepoName,
                inputs.BranchName,
                inputs.FilePath);

            this.consoleService.Write("data retrieved", true, 2);
            this.consoleService.Write("✅️Validating version keys . . . ");
            var keyValues = new List<string>();
            var versionKeys = inputs.VersionKeys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (versionKeys.Length <= 0)
            {
                throw new NoVersionFoundException("No version keys supplied for the 'version-keys' input.");
            }

            this.consoleService.Write("version keys validated.", true, 2);
            this.consoleService.Write("✅️Pulling version from file . . . ");

            foreach (var versionKey in versionKeys)
            {
                var keyExists = this.xmlParserService.KeyExists(fileData, versionKey, inputs.CaseSensitiveKeys ?? false);

                var keyValue = keyExists
                    ? this.xmlParserService.GetKeyValue(fileData, versionKey)
                    : string.Empty;
                keyValues.Add(keyValue);
            }

            this.consoleService.Write("version pulled from file.", true, 2);

            FailIfMismatch(inputs.FailOnKeyValueMismatch, keyValues);

            // Just get the first value that is not empty
            var version = keyValues.Find(v => !string.IsNullOrEmpty(v)) ?? string.Empty;

            if ((inputs.FailWhenVersionNotFound ?? false) && string.IsNullOrEmpty(version))
            {
                var exceptionMsg = "No version value was found.";
                exceptionMsg += $"{Environment.NewLine}If you do not want the GitHub action to fail when no version is found,";
                exceptionMsg += "set the 'fail-when-version-not-found' input to a value of 'false'.";

                throw new NoVersionFoundException(exceptionMsg);
            }

            this.consoleService.StartGroup("Version Miner Outputs");
            this.consoleService.WriteLine($"version: {version}");
            this.consoleService.EndGroup();
            this.consoleService.BlankLine();

            this.actionOutputService.SetOutputValue(VersionOutputName, version);
        }
        catch (Exception e)
        {
            onError(e);
        }

        onCompleted();
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Nothing to dispose of.")]
    public void Dispose()
    {
        // Enforced by interface but nothing to dispose of
    }

    /// <summary>
    /// Fails the action if set to fail on key value mismatch, and if any of the key values do not match.
    /// </summary>
    /// <param name="failOnKeyValueMismatch">True to fail on mismatch.</param>
    /// <param name="keyValues">The key values pulled from the file.</param>
    /// <exception cref="ValuesMismatchException">
    ///     Thrown if any of the values are a mismatch.
    /// </exception>
    private static void FailIfMismatch(bool? failOnKeyValueMismatch, List<string> keyValues)
    {
        // If the action should fail on key value mismatch
        if (!(failOnKeyValueMismatch ?? false) || keyValues.TrueForAll(v => v == keyValues[0]))
        {
            return;
        }

        var exceptionMsg = "All values must match.";
        exceptionMsg += "This failure only occurs if the 'fail-on-key-value-mismatch' action input is set to 'true'.";

        throw new ValuesMismatchException(exceptionMsg);
    }

    /// <summary>
    /// Sets up the token for authorized requests if a token exists.
    /// </summary>
    /// <param name="repoToken">The token to use for authorization.</param>
    private void SetupToken(string? repoToken)
    {
        if (string.IsNullOrEmpty(repoToken))
        {
            return;
        }

        var tokenAuth = new Credentials(repoToken);
        this.githubClient.Connection.Credentials = tokenAuth;
    }

    /// <summary>
    /// Gets the repository if the repository exists and throws an exception if it does not.
    /// </summary>
    /// <param name="repoOwner">The owner of the repository.</param>
    /// <param name="repoName">The name of the repository.</param>
    /// <returns> The asynchronous result of the repository if it exists.</returns>
    /// <exception cref="RepoDoesNotExistException">Thrown if not authorized to perform the request.</exception>
    /// <exception cref="NotFoundException">Thrown if the repository does not exist.</exception>
    private async Task<Repository> GetIfRepoExists(string repoOwner, string repoName)
    {
        try
        {
            return await this.githubClient.Repository.Get(repoOwner, repoName);
        }
        catch (Exception ex) when (ex is AuthorizationException exception)
        {
            this.consoleService.WriteError($"{exception.StatusCode} - {exception.Message}");
            throw;
        }
        catch (Exception ex) when (ex is NotFoundException)
        {
            this.consoleService.WriteError($"The repository owner '${repoOwner}' and/or the repository '{repoName}' does not exist.");
            throw new RepoDoesNotExistException($"The repository owner '{repoOwner}' and/or the repository '{repoName}' does not exist.");
        }
    }

    /// <summary>
    /// Logs to the console that the branch has been trimmed if the branch needs to be trimmed.
    /// </summary>
    /// <param name="inputs">The incoming action inputs.</param>
    private void LogIfBranchNeedsTrimming(ActionInputs inputs)
    {
        var branchNeedsTrimming = string.IsNullOrEmpty(inputs.TrimStartFromBranch) is false &&
                                  inputs.BranchName.ToLower().StartsWith(inputs.TrimStartFromBranch.ToLower());

        if (!branchNeedsTrimming)
        {
            return;
        }

        this.consoleService.WriteLine($"Branch Before Trimming: {inputs.BranchName}");

        inputs.BranchName = inputs.BranchName.TrimStart(inputs.TrimStartFromBranch);

        this.consoleService.WriteLine($"The text '{inputs.TrimStartFromBranch}' has been trimmed from the branch name.");
        this.consoleService.WriteLine($"Branch After Trimming: {inputs.BranchName}");
        this.consoleService.BlankLine();
    }

    /// <summary>
    /// Shows a welcome message with additional information.
    /// </summary>
    private void ShowWelcomeMessage()
    {
        this.consoleService.WriteLine("Welcome to Version Miner!! 🪨⛏️");
        this.consoleService.WriteLine("A GitHub action for pulling versions out of various types of files.");
        this.consoleService.WriteLine($"To open an issue, click here 👉🏼 https://github.com/KinsonDigital/VersionMiner/issues/new/choose");
        this.consoleService.BlankLine();
        this.consoleService.BlankLine();
    }
}
