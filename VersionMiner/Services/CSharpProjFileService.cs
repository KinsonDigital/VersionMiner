﻿// <copyright file="CSharpProjFileService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using VersionMiner.Guards;

namespace VersionMiner.Services;

/// <summary>
/// Performs operations on data in C# project files.
/// </summary>
public class CSharpProjFileService
{
    private const string VersionElementName = "Version";
    private const string FileVersionElementName = "FileVersion";
    private const string AssemblyVersionElementName = "AssemblyVersion";
    private readonly IDataParserService xmlParserService;

    /// <summary>
    /// Initializes a new instance of the <see cref="CSharpProjFileService"/> class.
    /// </summary>
    /// <param name="xmlParserService">Parses XML data in C# project files.</param>
    public CSharpProjFileService(IDataParserService xmlParserService)
    {
        EnsureThat.CtorParamIsNotNull(xmlParserService);
        this.xmlParserService = xmlParserService;
    }

    /// <summary>
    /// Gets the version from the <i><c>Version</c></i> element.
    /// </summary>
    /// <param name="projFileData">The C# project file data.</param>
    /// <returns>
    ///     Returns a tuple with a <i><b>bool</b></i> value indicating whether
    ///     or not the <i><c>Version</c></i> element exists.
    ///     If the element does exists, the version is returned.
    /// </returns>
    public (bool exists, string version) GetVersion(string projFileData)
    {
        var keyExists = this.xmlParserService.KeyExists(projFileData, VersionElementName);

        if (keyExists is false)
        {
            return (false, string.Empty);
        }

        var version = this.xmlParserService.GetKeyValue(projFileData, VersionElementName);

        return (true, version);
    }

    /// <summary>
    /// Gets the file version from the <i><c>FileVersion</c></i> element.
    /// </summary>
    /// <param name="projFileData">The C# project file data.</param>
    /// <returns>
    ///     Returns a tuple with a <i><b>bool</b></i> value indicating whether
    ///     or not the <i><c>FileVersion</c></i> element exists.
    ///     If the element does exists, the version is returned.
    /// </returns>
    public (bool exists, string version) GetFileVersion(string projFileData)
    {
        var keyExists = this.xmlParserService.KeyExists(projFileData, FileVersionElementName);

        if (keyExists is false)
        {
            return (false, string.Empty);
        }

        var version = this.xmlParserService.GetKeyValue(projFileData, FileVersionElementName);

        return (true, version);
    }

    /// <summary>
    /// Gets the file version from the <i><c>AssemblyVersion</c></i> element.
    /// </summary>
    /// <param name="projFileData">The C# project file data.</param>
    /// <returns>
    ///     Returns a tuple with a <i><b>bool</b></i> value indicating whether
    ///     or not the <i><c>AssemblyVersion</c></i> element exists.
    ///     If the element does exists, the version is returned.
    /// </returns>
    public (bool exists, string version) GetAssemblyVersion(string projFileData)
    {
        var keyExists = this.xmlParserService.KeyExists(projFileData, AssemblyVersionElementName);

        if (keyExists is false)
        {
            return (false, string.Empty);
        }

        var version = this.xmlParserService.GetKeyValue(projFileData, AssemblyVersionElementName);

        return (keyExists, version);
    }
}
