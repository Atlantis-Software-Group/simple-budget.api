using asg.dbmigrator;
using asg.dbmigrator.CommandLineParser;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using simple_budget.api.data;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                .WriteTo.Console()
                .CreateBootstrapLogger();

        Log.Information("Args: {@args}", args);
        CommandLineArgParser parser = new CommandLineArgParser(args);
        CreateCommandLineOptions(parser);

        ICommandLineArgs parsedArgs = parser.Parse();
        Log.Information("Parsed Args: {@parsedArgs}", parsedArgs);

        try
        {
            Log.Information("Configuring Host");
            IHostBuilder builder = Host.CreateDefaultBuilder();

            string environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Local";
            Log.Information("Environment: {environment}", environment);

            if (parsedArgs.GetValue<bool>("-ef") && string.Equals(environment, "Local", StringComparison.OrdinalIgnoreCase))
                builder.UseEnvironment("Development");


            builder.UseSerilog((ctx, services, configuration) =>
            {
                IConfiguration config = ctx.Configuration;
                string seqUrl = config["Seq:ServerUrl"] ?? string.Empty;
                Log.Information("Seq URL: {url}", seqUrl);
                configuration
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Debug)
                    .Enrich.WithProperty("Application", "Simple-Budget.Api.Data.DatabaseMigrator")
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .WriteTo.Seq(seqUrl)
                    .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
            });

            builder.ConfigureServices((ctx, services) =>
            {                        
                services.AddDbMigratorHostedService((options) => {
                    options.ParsedArgs = parsedArgs;
                    options.DbContextTypes = new Type[] {
                        typeof(ApplicationDbContext),
                    };
                });

                var TransactionDbConnectionString = ctx.Configuration.GetConnectionString("TransactionDb");

                services.AddDbContext<ApplicationDbContext>(options =>
                {                    
                    options.UseSqlServer(TransactionDbConnectionString, dbOpts => dbOpts.MigrationsAssembly(typeof(Program).Assembly.FullName));
                    options.AddInternalServiceProvider();
                });
            });

            IHost host = builder.Build();

            if (parsedArgs.GetValue<bool>("-migrate"))
            {
                Log.Information("Starting Host");
                await host.StartAsync();

                await host.WaitForShutdownAsync();
                Log.Information("Host shutdown successfully.");
            }
            else
            {
                Log.Information("Migration not requested");
            }
            
        }
        catch(Exception e)
        {
            Log.Fatal(e, "Host Terminated unexpectedly.");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }

        return 0;
    }

    private static void CreateCommandLineOptions(CommandLineArgParser parser)
    {
        CommandLineOption<bool> migrateOption = new CommandLineOption<bool>("migrate");
        parser.Add(migrateOption.Name, migrateOption);

        CommandLineOption<bool> efOption = new CommandLineOption<bool>("ef");
        parser.Add(efOption.Name, efOption);

        CommandLineOption<bool> createSeedScriptOption = new CommandLineOption<bool>("createSeedScript");
        parser.Add(createSeedScriptOption.Name, createSeedScriptOption);

        CommandLineOption<string> scriptNameOption = new CommandLineOption<string>("scriptName");
        parser.Add(scriptNameOption.Name, scriptNameOption);

        CommandLineOption<string> dbContextNameOption = new CommandLineOption<string>("dbContextName");
        parser.Add(dbContextNameOption.Name, dbContextNameOption);

        CommandLineOption<string> migrationNameOption = new CommandLineOption<string>("migrationName");
        parser.Add(migrationNameOption.Name, migrationNameOption);

        CommandLineOption<string, List<string>> environmentNamesOption = new CommandLineOption<string, List<string>>("environmentNames");
        parser.Add(environmentNamesOption.Name, environmentNamesOption);
    }
}