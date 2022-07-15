// <copyright file="BranchModel.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace VersionMiner.Models;

/// <summary>
/// Holds data about a repository branch.
/// </summary>
public record BranchModel
{
    /// <summary>
    /// Gets or sets the name of the branch.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
