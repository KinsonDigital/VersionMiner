// <copyright file="GitHubAction.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using VersionMiner.Exceptions;
using VersionMiner.Guards;
using VersionMiner.Services;

namespace VersionMiner;

/// <inheritdoc/>
public sealed class GitHubAction : IGitHubAction
{
    private const string VersionOutputName = "version";
    private const string XMLFileFormat = "xml";
    private readonly IGitHubConsoleService gitHubConsoleService;
    private readonly IGitHubDataService gitHubDataService;
    private readonly IDataParserService xmlParserService;
    private readonly IActionOutputService actionOutputService;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GitHubAction"/> class.
    /// </summary>
    /// <param name="gitHubConsoleService">Writes to the console.</param>
    /// <param name="gitHubDataService">Provides data access to GitHub.</param>
    /// <param name="xmlParserService">Parses XML data.</param>
    /// <param name="actionOutputService">Sets the output data of the action.</param>
    public GitHubAction(
        IGitHubConsoleService gitHubConsoleService,
        IGitHubDataService gitHubDataService,
        IDataParserService xmlParserService,
        IActionOutputService actionOutputService)
    {
        EnsureThat.CtorParamIsNotNull(gitHubConsoleService);
        EnsureThat.CtorParamIsNotNull(gitHubDataService);
        EnsureThat.CtorParamIsNotNull(xmlParserService);
        EnsureThat.CtorParamIsNotNull(actionOutputService);

        this.gitHubConsoleService = gitHubConsoleService;
        this.gitHubDataService = gitHubDataService;
        this.xmlParserService = xmlParserService;
        this.actionOutputService = actionOutputService;
    }

    /// <inheritdoc/>
    public async Task Run(ActionInputs inputs, Action onCompleted, Action<Exception> onError)
    {
        ShowWelcomeMessage();

        this.gitHubDataService.RepoOwner = inputs.RepoOwner;
        this.gitHubDataService.RepoName = inputs.RepoName;
        this.gitHubDataService.BranchName = inputs.BranchName;
        this.gitHubDataService.FilePath = inputs.FilePath;

        try
        {
            if (inputs.FileFormat != XMLFileFormat)
            {
                var exMsg = $"The file type value of '{inputs.FileFormat}' is invalid.";
                exMsg += $"{Environment.NewLine}The only file type currently supported are csproj files.";
                throw new InvalidFileTypeException(exMsg);
            }

            this.gitHubConsoleService.Write($"✔️️Verifying if the repository owner '{inputs.RepoOwner}' exists . . . ");
            var ownerExists = await this.gitHubDataService.OwnerExists();
            if (ownerExists is false)
            {
                throw new OwnerDoesNotExistException($"The repository owner '{inputs.RepoOwner}' does not exist.");
            }

            this.gitHubConsoleService.Write("the owner exists.", true);
            this.gitHubConsoleService.BlankLine();

            this.gitHubConsoleService.Write($"✔️️Verifying if the repository '{inputs.RepoName}' exists . . . ");
            var repoExists = await this.gitHubDataService.RepoExists();
            if (repoExists is false)
            {
                throw new RepoDoesNotExistException($"The repository '{inputs.RepoName}' does not exist.");
            }

            this.gitHubConsoleService.Write("the repository exists.", true);
            this.gitHubConsoleService.BlankLine();

            this.gitHubConsoleService.Write($"✔️️Verifying if the repository branch '{inputs.BranchName}' exists . . . ");
            var branchExists = await this.gitHubDataService.BranchExists();
            if (branchExists is false)
            {
                throw new BranchDoesNotExistException($"The repository branch '{inputs.BranchName}' does not exist.");
            }

            this.gitHubConsoleService.Write("the branch exists.", true);
            this.gitHubConsoleService.BlankLine();

            this.gitHubConsoleService.Write($"✔️️Getting data for file '{inputs.FilePath}' . . . ");
            var xmlData = await this.gitHubDataService.GetFileData();
            this.gitHubConsoleService.Write("data retrieved", true);
            this.gitHubConsoleService.BlankLine();

            this.gitHubConsoleService.Write("✔️️Validating version keys . . . ");
            var keyValues = new List<string>();
            var versionKeys = inputs.VersionKeys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (versionKeys.Length <= 0)
            {
                throw new NoVersionFoundException("No version keys supplied for the 'version-keys' input.");
            }

            this.gitHubConsoleService.Write("version keys validated.", true);
            this.gitHubConsoleService.BlankLine();

            this.gitHubConsoleService.Write("✔️️Pulling version from file . . . ");
            foreach (var versionKey in versionKeys)
            {
                var keyExists = this.xmlParserService.KeyExists(xmlData, versionKey, inputs.CaseSensitiveKeys);

                var keyValue = keyExists
                    ? this.xmlParserService.GetKeyValue(xmlData, versionKey)
                    : string.Empty;
                keyValues.Add(keyValue);
            }

            this.gitHubConsoleService.Write("version pulled from file.", true);
            this.gitHubConsoleService.BlankLine();

            /* If the action should fail on key value mismatch,
             * collect all of the values for comparison.
            */
            if (inputs.FailOnKeyValueMismatch)
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

            if (inputs.FailWhenVersionNotFound && string.IsNullOrEmpty(version))
            {
                var exceptionMsg = "No version value was found.";
                exceptionMsg += $"{Environment.NewLine}If you do not want the GitHub action to fail when no version is found,";
                exceptionMsg += "set the 'fail-when-version-not-found' input to a value of 'false'.";

                throw new NoVersionFoundException(exceptionMsg);
            }

            this.gitHubConsoleService.StartGroup("Version Miner Outputs");
            this.gitHubConsoleService.WriteLine($"version: {version}");
            this.gitHubConsoleService.EndGroup();
            this.gitHubConsoleService.BlankLine();

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
        this.gitHubConsoleService.WriteLine("Welcome to Version Miner!! 🪨⛏️");
        this.gitHubConsoleService.WriteLine("A GitHub action for pulling versions out of various types of files.");
        this.gitHubConsoleService.BlankLine();
        this.gitHubConsoleService.BlankLine();
    }
}
