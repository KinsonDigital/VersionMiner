// <copyright file="GitHubAction.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using GitHubData.Services;
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
    private readonly IGitHubDataService gitHubDataService;
    private readonly IDataParserService xmlParserService;
    private readonly IActionOutputService actionOutputService;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubAction"/> class.
    /// </summary>
    /// <param name="consoleService">Writes to the console.</param>
    /// <param name="gitHubDataService">Provides data access to GitHub.</param>
    /// <param name="xmlParserService">Parses XML data.</param>
    /// <param name="actionOutputService">Sets the output data of the action.</param>
    public GitHubAction(
        IGitHubConsoleService consoleService,
        IGitHubDataService gitHubDataService,
        IDataParserService xmlParserService,
        IActionOutputService actionOutputService)
    {
        EnsureThat.CtorParamIsNotNull(consoleService);
        EnsureThat.CtorParamIsNotNull(gitHubDataService);
        EnsureThat.CtorParamIsNotNull(xmlParserService);
        EnsureThat.CtorParamIsNotNull(actionOutputService);

        this.consoleService = consoleService;
        this.gitHubDataService = gitHubDataService;
        this.xmlParserService = xmlParserService;
        this.actionOutputService = actionOutputService;
    }

    /// <inheritdoc/>
    public async Task Run(ActionInputs inputs, Action onCompleted, Action<Exception> onError)
    {
        ShowWelcomeMessage();

        var branchNeedsTrimming = string.IsNullOrEmpty(inputs.TrimStartFromBranch) is false &&
                                  inputs.BranchName.ToLower().StartsWith(inputs.TrimStartFromBranch.ToLower());

        if (branchNeedsTrimming)
        {
            this.consoleService.WriteLine($"Branch Before Trimming: {inputs.BranchName}");

            inputs.BranchName = inputs.BranchName.TrimStart(inputs.TrimStartFromBranch);

            this.consoleService.WriteLine($"The text '{inputs.TrimStartFromBranch}' has been trimmed from the branch name.");
            this.consoleService.WriteLine($"Branch After Trimming: {inputs.BranchName}");
            this.consoleService.BlankLine();
        }

        try
        {
            if (inputs.FileFormat.ToLower() != XMLFileFormat)
            {
                var exMsg = $"The 'file-format' value of '{inputs.FileFormat}' is invalid.";
                exMsg += $"{Environment.NewLine}The only file format currently supported is XML.";
                throw new InvalidFileFormatException(exMsg);
            }

            // If a repo token exists
            if (string.IsNullOrEmpty(inputs.RepoToken) is false)
            {
                this.gitHubDataService.AuthToken = inputs.RepoToken;
            }

            this.consoleService.Write($"✔️️Verifying if the repository owner '{inputs.RepoOwner}' exists . . . ");
            var ownerExists = await this.gitHubDataService.OwnerExistsAsync(
                inputs.RepoOwner);

            if (ownerExists is false)
            {
                throw new OwnerDoesNotExistException($"The repository owner '{inputs.RepoOwner}' does not exist.");
            }

            this.consoleService.Write("the owner exists.", true);
            this.consoleService.BlankLine();

            this.consoleService.Write($"✔️️Verifying if the repository '{inputs.RepoName}' exists . . . ");
            var repoExists = await this.gitHubDataService.RepoExistsAsync(
                inputs.RepoOwner,
                inputs.RepoName);

            if (repoExists is false)
            {
                throw new RepoDoesNotExistException($"The repository '{inputs.RepoName}' does not exist.");
            }

            this.consoleService.Write("the repository exists.", true);
            this.consoleService.BlankLine();

            this.consoleService.Write($"✔️️Verifying if the repository branch '{inputs.BranchName}' exists . . . ");
            var branchExists = await this.gitHubDataService.BranchExistsAsync(
                inputs.RepoOwner,
                inputs.RepoName,
                inputs.BranchName);

            if (branchExists is false)
            {
                throw new BranchDoesNotExistException($"The repository branch '{inputs.BranchName}' does not exist.");
            }

            this.consoleService.Write("the branch exists.", true);
            this.consoleService.BlankLine();

            this.consoleService.Write($"✔️️Getting data for file '{inputs.FilePath}' . . . ");
            var xmlData = await this.gitHubDataService.GetFileDataAsync(
                inputs.RepoOwner,
                inputs.RepoName,
                inputs.BranchName,
                inputs.FilePath);

            this.consoleService.Write("data retrieved", true);
            this.consoleService.BlankLine();

            this.consoleService.Write("✔️️Validating version keys . . . ");
            var keyValues = new List<string>();
            var versionKeys = inputs.VersionKeys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (versionKeys.Length <= 0)
            {
                throw new NoVersionFoundException("No version keys supplied for the 'version-keys' input.");
            }

            this.consoleService.Write("version keys validated.", true);
            this.consoleService.BlankLine();

            this.consoleService.Write("✔️️Pulling version from file . . . ");
            foreach (var versionKey in versionKeys)
            {
                var keyExists = this.xmlParserService.KeyExists(xmlData, versionKey, inputs.CaseSensitiveKeys ?? false);

                var keyValue = keyExists
                    ? this.xmlParserService.GetKeyValue(xmlData, versionKey)
                    : string.Empty;
                keyValues.Add(keyValue);
            }

            this.consoleService.Write("version pulled from file.", true);
            this.consoleService.BlankLine();

            /* If the action should fail on key value mismatch,
             * collect all of the values for comparison.
            */
            if (inputs.FailOnKeyValueMismatch ?? false)
            {
                if (keyValues.TrueForAll(v => v == keyValues[0]) is false)
                {
                    var exceptionMsg = "All values must match.";
                    exceptionMsg += "This failure only occurs if the 'fail-on-key-value-mismatch' action input is set to 'true'.";

                    throw new ValuesMismatchException(exceptionMsg);
                }
            }

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
    public void Dispose() => Dispose(true);

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.gitHubDataService.Dispose();
        }

        this.isDisposed = true;
    }

    /// <summary>
    /// Shows a welcome message with additional information.
    /// </summary>
    private void ShowWelcomeMessage()
    {
        var issueUrl = "https://github.com/KinsonDigital/VersionMiner/issues/new/choose";
        this.consoleService.WriteLine("Welcome to Version Miner!! 🪨⛏️");
        this.consoleService.WriteLine("A GitHub action for pulling versions out of various types of files.");
        this.consoleService.WriteLine($"To open an issue, click here 👉🏼 {issueUrl}");
        this.consoleService.BlankLine();
        this.consoleService.BlankLine();
    }
}
