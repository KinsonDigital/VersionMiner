// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Octokit;
using VersionMiner.Services;
using MinerHttpClient = VersionMiner.Services.HttpClient;

namespace VersionMiner;

/// <summary>
/// The main application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    private static readonly FileSystem FileSystem = new ();
    private static IHost host = null!;

    /// <summary>
    /// The main entry point of the GitHub action.
    /// </summary>
    /// <param name="args">The incoming arguments(action inputs).</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IGitHubClient, GitHubClient>(_
                    => new GitHubClient(new ProductHeaderValue("version-miner", App.GetVersion())));
                services.AddSingleton<IRepositoryContentsClient>(provider =>
                {
                    var service = provider.GetService<IGitHubClient>();

                    if (service is null)
                    {
                        throw new InvalidOperationException($"The service '{nameof(IGitHubClient)}' cannot be null when resolving dependencies.");
                    }

                    return service.Repository.Content;
                });
                services.AddSingleton<IAppService, AppService>();
                services.AddSingleton<IHttpClient, MinerHttpClient>();
                services.AddSingleton<IGitHubConsoleService, GitHubConsoleService>();
                services.AddSingleton<IRepoFileDataService, RepoFileDataService>();
                services.AddSingleton<IActionOutputService, ActionOutputService>();
                services.AddSingleton<IDataParserService, XMLParserService>();
                services.AddSingleton<IArgParsingService<ActionInputs>, ArgParsingService>();
                services.AddSingleton<IGitHubAction, GitHubAction>();
                services.AddSingleton<IEnvVarService, EnvVarService>();
                services.AddSingleton(FileSystem.File);
            }).Build();

        IAppService appService;
        IGitHubAction gitHubAction;
        IGitHubConsoleService consoleService;

        try
        {
            appService = host.Services.GetRequiredService<IAppService>();
            gitHubAction = host.Services.GetRequiredService<IGitHubAction>();
            consoleService = host.Services.GetRequiredService<IGitHubConsoleService>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        var argParsingService = host.Services.GetRequiredService<IArgParsingService<ActionInputs>>();

        await argParsingService.ParseArguments(
            new ActionInputs(),
            args,
            async inputs =>
            {
                await gitHubAction.Run(
                    inputs,
                    () =>
                    {
                        host.Dispose();
                        Default.Dispose();
                        gitHubAction.Dispose();
                        appService.Exit(0);
                    },
                    e =>
                    {
                        host.Dispose();
                        Default.Dispose();
                        gitHubAction.Dispose();
                        appService.ExitWithException(e);
                    });
            }, errors =>
            {
                foreach (var error in errors)
                {
                    consoleService.WriteLine(error);
                }

                appService.ExitWithException(new Exception($"There were {errors.Length} errors.  Refer to the logs for more information."));
            });
    }
}
