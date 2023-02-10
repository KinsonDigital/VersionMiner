// <copyright file="IntegrationTestsBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Reflection;

namespace VersionMinerIntegrationTests;

/// <summary>
/// Provides a base class for integration tests.
/// </summary>
public abstract class IntegrationTestsBase
{
    private const string EnvOutputFile = "set_env_output.txt";
    private const string GitHubOutput = "GITHUB_OUTPUT";
    private readonly string baseDirPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}";

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestsBase"/> class.
    /// </summary>
    protected IntegrationTestsBase()
    {
        // Setup the environment by setting up the env output file and environment variable
        var fullFilePath = $"{this.baseDirPath}/{EnvOutputFile}";

        try
        {
            if (File.Exists(fullFilePath))
            {
                File.Delete(fullFilePath);
            }

            File.WriteAllText(fullFilePath, string.Empty);

            Environment.SetEnvironmentVariable(GitHubOutput, fullFilePath, EnvironmentVariableTarget.Process);
        }
        catch (Exception e)
        {
            var message = "Problem setting up environment for integration tests.";
            message += $"{Environment.NewLine}{e.Message}";

            Assert.True(false, message);
        }
    }
}
