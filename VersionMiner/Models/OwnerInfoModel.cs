// <copyright file="OwnerInfoModel.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace VersionMiner.Models;

/// <summary>
/// Holds data about a repository owner.
/// </summary>
public record OwnerInfoModel
{
    /// <summary>
    /// Gets or sets the login name of the owner.
    /// </summary>
    public string Login { get; set; } = string.Empty;
}
