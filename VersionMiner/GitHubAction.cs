// <copyright file="GitHubAction.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using VersionMiner.Exceptions;
using VersionMiner.Services;

namespace VersionMiner;

/// <inheritdoc/>
public sealed class GitHubAction : IGitHubAction
{
    private const string VersionOutputName = "version";
    private const string XMLFileFormat = "xml";
    private readonly IGitHubConsoleService _gitHubConsoleService;
    private readonly IGitHubDataService _gitHubDataService;
    private readonly IDataParserService _xmlParserService;
    private readonly IActionOutputService _actionOutputService;
    private bool _isDisposed;

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
        _gitHubConsoleService = gitHubConsoleService;
        _gitHubDataService = gitHubDataService;
        _xmlParserService = xmlParserService;
        _actionOutputService = actionOutputService;
    }

    /// <inheritdoc/>
    public async Task Run(ActionInputs inputs, Action onCompleted, Action<Exception> onError)
    {
        ShowWelcomeMessage();

        _gitHubDataService.RepoOwner = inputs.RepoOwner;
        _gitHubDataService.RepoName = inputs.RepoName;
        _gitHubDataService.BranchName = inputs.BranchName;
        _gitHubDataService.FilePath = inputs.FilePath;

        try
        {
            if (inputs.FileFormat != XMLFileFormat)
            {
                var exMsg = $"The file type value of '{inputs.FileFormat}' is invalid.";
                exMsg += $"{Environment.NewLine}The only file type currently supported are csproj files.";
                throw new InvalidFileTypeException(exMsg);
            }

            _gitHubConsoleService.Write($"✔️️Verifying if the repository owner '{inputs.RepoOwner}' exists . . . ");
            var ownerExists = await _gitHubDataService.OwnerExists();
            if (ownerExists is false)
            {
                throw new OwnerDoesNotExistException($"The repository owner '{inputs.RepoOwner}' does not exist.");
            }

            _gitHubConsoleService.Write("the owner exists.", true);
            _gitHubConsoleService.BlankLine();

            _gitHubConsoleService.Write($"✔️️Verifying if the repository '{inputs.RepoName}' exists . . . ");
            var repoExists = await _gitHubDataService.RepoExists();
            if (repoExists is false)
            {
                throw new RepoDoesNotExistException($"The repository '{inputs.RepoName}' does not exist.");
            }

            _gitHubConsoleService.Write("the repository exists.", true);
            _gitHubConsoleService.BlankLine();

            _gitHubConsoleService.Write($"✔️️Verifying if the repository branch '{inputs.BranchName}' exists . . . ");
            var branchExists = await _gitHubDataService.BranchExists();
            if (branchExists is false)
            {
                throw new BranchDoesNotExistException($"The repository branch '{inputs.BranchName}' does not exist.");
            }

            _gitHubConsoleService.Write("the branch exists.", true);
            _gitHubConsoleService.BlankLine();

            _gitHubConsoleService.Write($"✔️️Getting data for file '{inputs.FilePath}' . . . ");
            var xmlData = await _gitHubDataService.GetFileData();
            _gitHubConsoleService.Write("data retrieved", true);
            _gitHubConsoleService.BlankLine();

            _gitHubConsoleService.Write("✔️️Validating version keys . . . ");
            var keyValues = new List<string>();
            var versionKeys = inputs.VersionKeys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (versionKeys.Length <= 0)
            {
                throw new NoVersionFoundException("No version keys supplied for the 'version-keys' input.");
            }

            _gitHubConsoleService.Write("version keys validated.", true);
            _gitHubConsoleService.BlankLine();

            _gitHubConsoleService.Write("✔️️Pulling version from file . . . ");
            foreach (var versionKey in versionKeys)
            {
                var keyExists = _xmlParserService.KeyExists(xmlData, versionKey, inputs.CaseSensitiveKeys);

                var keyValue = keyExists
                    ? _xmlParserService.GetKeyValue(xmlData, versionKey)
                    : string.Empty;
                keyValues.Add(keyValue);
            }

            _gitHubConsoleService.Write("version pulled from file.", true);
            _gitHubConsoleService.BlankLine();

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

            _gitHubConsoleService.StartGroup("Version Miner Outputs");
            _gitHubConsoleService.WriteLine($"version: {version}");
            _gitHubConsoleService.EndGroup();
            _gitHubConsoleService.BlankLine();

            _actionOutputService.SetOutputValue(VersionOutputName, version);
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
    /// <param name="disposing">True to dispose of managed resources.</param>
    private void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _gitHubDataService.Dispose();
        }

        _isDisposed = true;
    }

    /// <summary>
    /// Shows a welcome message with additional information.
    /// </summary>
    private void ShowWelcomeMessage()
    {
        _gitHubConsoleService.WriteLine("Welcome to Version Miner!! 🪨⛏️");
        _gitHubConsoleService.WriteLine("A GitHub action for pulling versions out of various types of files!!");
        _gitHubConsoleService.BlankLine();
        _gitHubConsoleService.BlankLine();
    }
}
