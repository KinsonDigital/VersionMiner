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
                var tokenAuth = new Credentials(inputs.RepoToken);
                this.githubClient.Connection.Credentials = tokenAuth;
            }

            this.consoleService.Write($"✔️️Verifying if the repository '{inputs.RepoName}' exists . . . ");

            Repository? repo;

            this.consoleService.Write("the repository exists.", true, 2);
            {
                repo = await this.githubClient.Repository.Get(inputs.RepoOwner, inputs.RepoName);

                if (repo is null)
                {
                    throw new RepoDoesNotExistException($"The repository '{inputs.RepoName}' does not exist.");
                }
            }
            catch (Exception ex) when (ex is AuthorizationException)
            {
                this.consoleService.WriteError($"{((AuthorizationException)ex).StatusCode} - {ex.Message}");
                throw;
            }
            catch (Exception ex) when (ex is NotFoundException)
            {
                this.consoleService.WriteError($"The repository owner '${inputs.RepoOwner}' and/or the repository '{inputs.RepoName}' does not exist.");
                throw;
            }

            this.consoleService.Write("the repository exists.", true);
            this.consoleService.BlankLine();

            this.consoleService.Write($"✔️️Verifying if the repository branch '{inputs.BranchName}' exists . . . ");

            var branch = await this.githubClient.Repository.Branch.Get(repo.Id, inputs.BranchName);

            if (branch is null)
            {
                throw new HttpRequestException($"The repository branch '{inputs.BranchName}' does not exist.");
            }

            this.consoleService.Write("the branch exists.", true, 2);
            this.consoleService.Write($"✔️️Getting data for file '{inputs.FilePath}' . . . ");
            this.repoFileDataService.AuthToken = inputs.RepoToken;

            var fileData = await this.repoFileDataService.GetFileData(
                inputs.RepoOwner,
                inputs.RepoName,
                inputs.BranchName,
                inputs.FilePath);

            this.consoleService.Write("data retrieved", true, 2);
            this.consoleService.Write("✔️️Validating version keys . . . ");
            var keyValues = new List<string>();
            var versionKeys = inputs.VersionKeys.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (versionKeys.Length <= 0)
            {
                throw new NoVersionFoundException("No version keys supplied for the 'version-keys' input.");
            }

            this.consoleService.Write("version keys validated.", true, 2);
            this.consoleService.Write("✔️️Pulling version from file . . . ");
            foreach (var versionKey in versionKeys)
            {
                var keyExists = this.xmlParserService.KeyExists(fileData, versionKey, inputs.CaseSensitiveKeys ?? false);

                var keyValue = keyExists
                    ? this.xmlParserService.GetKeyValue(fileData, versionKey)
                    : string.Empty;
                keyValues.Add(keyValue);
            }

            this.consoleService.Write("version pulled from file.", true, 2);

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
    [ExcludeFromCodeCoverage(Justification = "Nothing to dispose of.")]
    public void Dispose()
    {
    }

    /// <summary>
    /// Shows a welcome message with additional information.
    /// </summary>
    private void ShowWelcomeMessage()
    {
        const string issueUrl = "https://github.com/KinsonDigital/VersionMiner/issues/new/choose";
        this.consoleService.WriteLine("Welcome to Version Miner!! 🪨⛏️");
        this.consoleService.WriteLine("A GitHub action for pulling versions out of various types of files.");
        this.consoleService.WriteLine($"To open an issue, click here 👉🏼 {issueUrl}");
        this.consoleService.BlankLine();
        this.consoleService.BlankLine();
    }
}
