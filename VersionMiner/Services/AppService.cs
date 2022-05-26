// <copyright file="AppService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using VersionMiner.Guards;

namespace VersionMiner.Services;

/// <inheritdoc/>
[ExcludeFromCodeCoverage]
public class AppService : IAppService
{
    private readonly IGitHubConsoleService _gitHubConsoleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppService"/> class.
    /// </summary>
    /// <param name="gitHubConsoleService">Writes to the console.</param>
    public AppService(IGitHubConsoleService gitHubConsoleService)
    {
        EnsureThat.CtorParamIsNotNull(gitHubConsoleService);
        _gitHubConsoleService = gitHubConsoleService;
    }

    /// <inheritdoc/>
    public void Exit(int code)
    {
#if DEBUG // Kept here to pause console for debugging purposes
        _gitHubConsoleService.PauseConsole();
#endif
        Environment.Exit(code);
    }

    /// <inheritdoc/>
    public void ExitWithNoError() => Exit(0);

    /// <inheritdoc/>
    public void ExitWithException(Exception exception)
    {
        _gitHubConsoleService.WriteError(exception.Message);
        Exit(exception.HResult);
    }
}
