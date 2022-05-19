namespace VersionMiner;

/// <summary>
/// Holds all of the GitHub actions inputs.
/// </summary>
public class ActionInputs
{
    /// <summary>
    /// Gets or sets the message.
    /// </summary>
    [Option('m', "message", Required = true, HelpText = "The message to print.")]
    public string Message { get; set; } = null!;
}
