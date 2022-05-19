using var host = Host.CreateDefaultBuilder(args).Build();

static TService Get<TService>(IHost host)
    where TService : notnull =>
    host.Services.GetRequiredService<TService>();

#pragma warning disable CS1998 Async function without await expression
static async Task RunActionAsync(ActionInputs inputs, IHost host)
{
    // TODO: Add specific action processing here

    // https://docs.github.com/actions/reference/workflow-commands-for-github-actions#setting-an-output-parameter
    Console.WriteLine("This is the VersionMiner GitHub action!!");
    Console.WriteLine($"This is the message: {inputs.Message}");
    Console.WriteLine($"::set-output name=my-output::sample-output");

#if DEBUG
    Console.ReadLine();
#endif

    Environment.Exit(0); // Exist with no failure
}
#pragma warning restore CS1998

var parser = Default.ParseArguments<ActionInputs>(() => new(), args);
parser.WithNotParsed(
    errors =>
    {
        // Display any issues with parsing the command line options
        Get<ILoggerFactory>(host)
            .CreateLogger("VersionMiner.Program")
            .LogError(string.Join(Environment.NewLine, errors.Select(error => error.ToString())));
        Environment.Exit(2);
    });

// If the command line values were successfully parsed, continue running
await parser.WithParsedAsync(options => RunActionAsync(options, host));
await host.RunAsync();
