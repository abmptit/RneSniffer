using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using Portal.Core.Data;

namespace RneSniffer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                // Création, configuration et démarrage de l'application
                CreateHostBuilder(args).Build().Run();
                Environment.ExitCode = (int)CodeRetour.Succes;
            }
            catch (Exception ex)
            {
                // Journalisation de l'exception non gérée et fin en erreur
                LogManager.GetCurrentClassLogger().Fatal(ex, "ERREUR FATALE");
                LogManager.GetCurrentClassLogger().Fatal(ex.ToString());
                Environment.ExitCode = (int)CodeRetour.Erreur;
            }
            LogManager.Shutdown();
        }
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var codeTraitement = "RNE_SNIFFER";
            return CreateDefaultHostBuilder(args, codeTraitement)
                .ConfigureServices((hostContext, services) =>
                {
                    // Timeout pour le shutdown du traitement (en cas d'arrêt avec SIGTERM ou CTRL-C de la console)
                    services.Configure<HostOptions>(options => options.ShutdownTimeout = TimeSpan.FromSeconds(10));
                    // Ajout de la classe implémentant le traitement (IHostedService)
                    // var connex = hostContext.Configuration.GetConnectionString("ConnexionApplicative");
                    var connectionString = "Data Source=rne.db";
                    services.AddDbContext<ApplicationDbContext>(
                        options => options.UseSqlite("rne.db"));
                    services.AddHostedService<MergeFileService>();
                })
                .UseConsoleLifetime();
        }
        private static IHostBuilder CreateDefaultHostBuilder(string[] args, string codeTraitement)
        {
            string prefixeVarEnvironnement = codeTraitement + "_";
            var builder = new HostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: prefixeVarEnvironnement);
                    if (args != null)
                    {
                        configHost.AddCommandLine(args);
                    }
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    var env = hostContext.HostingEnvironment;
                    configApp.AddJsonFile("appsettings.json", optional: true);
                    configApp.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
                    if (env.IsDevelopment() && !string.IsNullOrEmpty(env.ApplicationName))
                    {
                        var appAssembly = Assembly.Load(new AssemblyName(env.ApplicationName));
                        if (appAssembly != null)
                        {
                            configApp.AddUserSecrets(appAssembly, optional: true);
                        }
                    }
                    configApp.AddEnvironmentVariables(prefix: prefixeVarEnvironnement);
                    if (args != null)
                    {
                        configApp.AddCommandLine(args);
                    }
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
                });
            return builder;
        }
    }
}