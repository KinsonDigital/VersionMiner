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
                services.AddSingleton<IGitHubAction, GitHubAction>();
            }).Build();

        IAppService appService = null!;
        IGitHubAction gitHubAction = null!;

        try
        {
            appService = _host.Services.GetRequiredService<IAppService>();
            gitHubAction = _host.Services.GetRequiredService<IGitHubAction>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"::error::{e.Message}");
#if DEBUG
            Console.ReadLine();
#endif
            Environment.Exit(80);
        }

        try
        {
            var parser = Default.ParseArguments(() => new ActionInputs(), args);

            // Display and log any errors with the action inputs(arguments).
            parser.WithNotParsed(ProcessInputErrors);

            // If the command line values were successfully parsed, continue running
            await parser.WithParsedAsync(
                inputs => gitHubAction.Run(
                inputs,
                () =>
                {
                    _host.Dispose();
                    appService.Exit(0);
                },
                (e) =>
                {
                    _host.Dispose();
                    appService.ExitWithException(e);
                }));
        }
        catch (Exception e)
        {
            appService.ExitWithException(e);
        }
    }

    /// <summary>
    /// Processes any errors related to the action inputs.
    /// </summary>
    /// <param name="errors">The list of input errors.</param>
    /// <exception cref="NullReferenceException">
    ///     Thrown if the <see cref="IGitHubConsoleService"/> is null.
    /// </exception>
    private static void ProcessInputErrors(IEnumerable<Error> errors)
    {
        var appService = _host.Services.GetRequiredService<IAppService>();
        var consoleService = _host.Services.GetRequiredService<IGitHubConsoleService>();

        foreach (var error in errors)
        {
            if (error is UnknownOptionError unknownOptionError)
            {
                consoleService.WriteError($"Unknown action input with the name '{unknownOptionError.Token}'");
            }
            else if (error is MissingRequiredOptionError requiredOptionError)
            {
                consoleService.WriteError($"Missing action input '{requiredOptionError.NameInfo.LongName}'.  This input is required.");
            }
            else if (error is MissingValueOptionError missingValueOptionError)
            {
                consoleService.WriteError($"The action input '{missingValueOptionError.NameInfo.LongName}' has no value.");
            }
            else
            {
                consoleService.WriteError($"An action input error has occurred.{Environment.NewLine}{error}");
            }
        }

#if DEBUG
        Console.ReadLine();
#endif

        _host.Dispose();
        appService.Exit(90);
    }
}
