// <copyright file="ActionOutputService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using VersionMiner.Exceptions;

namespace VersionMiner.Services;

/// <inheritdoc/>
public class ActionOutputService : IActionOutputService
{
    private readonly IGitHubConsoleService _gitHubConsoleService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActionOutputService"/> class.
    /// </summary>
    /// <param name="gitHubConsoleService">Writes to the console.</param>
    public ActionOutputService(IGitHubConsoleService gitHubConsoleService) => _gitHubConsoleService = gitHubConsoleService;

    /// <inheritdoc/>
    public void SetOutputValue(string name, string value)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new NullOrEmptyStringException($"The parameter '{nameof(name)}' must not be null or empty.");
        }

        _gitHubConsoleService.WriteLine($"::set-output name={name}::{value}");
    }
}
