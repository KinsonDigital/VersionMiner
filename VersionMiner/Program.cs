// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using GitHubData;
using GitHubData.Services;
using VersionMiner.Services;
using HttpClient = GitHubData.HttpClient;

[assembly: InternalsVisibleTo("VersionMinerTests", AllInternalsVisible = true)]

namespace VersionMiner;

/// <summary>
/// The main application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
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
                services.AddSingleton<IAppService, AppService>();
                services.AddSingleton<IHttpClient, HttpClient>();
                services.AddSingleton<IJSONService, JSONService>();
                services.AddSingleton<IGitHubConsoleService, GitHubConsoleService>();
                services.AddSingleton<IRequestRateLimitService, RequestRateLimitService>();
                services.AddSingleton<IGitHubDataService, GitHubDataService>();
                services.AddSingleton<IActionOutputService, ActionOutputService>();
                services.AddSingleton<IDataParserService, XMLParserService>();
                services.AddSingleton<IArgParsingService<ActionInputs>, ArgParsingService>();
                services.AddSingleton<IGitHubAction, GitHubAction>();
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
