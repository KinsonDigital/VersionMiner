// <copyright file="Program.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using VersionMiner.Services;

[assembly: InternalsVisibleTo("VersionMinerTests", AllInternalsVisible = true)]

namespace VersionMiner;

/// <summary>
/// The main application.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Program
{
    private static IHost _host = null!;

    /// <summary>
    /// The main entry point of the GitHub action.
    /// </summary>
    /// <param name="args">The incoming arguments(action inputs).</param>
    /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
    public static async Task Main(string[] args)
    {
        _host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IAppService, AppService>();
                services.AddSingleton<IGitHubConsoleService, GitHubConsoleService>();
                services.AddSingleton<IGitHubDataService, GitHubDataService>();
                services.AddSingleton<IActionOutputService, ActionOutputService>();
                services.AddSingleton<IDataParserService, XMLParserService>();
                services.AddSingleton<IArgParsingService<ActionInputs>, ArgParsingService>();
                services.AddSingleton<IGitHubAction, GitHubAction>();
            }).Build();

        var appService = _host.Services.GetRequiredService<IAppService>();
        var gitHubAction = _host.Services.GetRequiredService<IGitHubAction>();
        var consoleService = _host.Services.GetRequiredService<IGitHubConsoleService>();

        var argParsingService = _host.Services.GetRequiredService<IArgParsingService<ActionInputs>>();

        await argParsingService.ParseArguments(
            new ActionInputs(),
            args,
            async inputs =>
            {
                await gitHubAction.Run(
                    inputs,
                    () =>
                    {
                        _host.Dispose();
                        Default.Dispose();
                        gitHubAction.Dispose();
                        appService.Exit(0);
                    },
                    (e) =>
                    {
                        _host.Dispose();
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
