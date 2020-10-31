using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RneSniffer
{
    public abstract class ServiceBase : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<ServiceBase> _logger;

        protected ServiceBase(IHostApplicationLifetime applicationLifetime, ILogger<ServiceBase> logger)
        {
            _applicationLifetime = applicationLifetime;
            _logger = logger;
        }

        public abstract Task ExecuterTraitementAsync(CancellationToken stoppingToken);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                try
                {
                    await ExecuterTraitementAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(1, ex, "Erreur fatale");
                    throw;
                }
                finally
                {
                    _applicationLifetime.StopApplication();
                }
            });
        }
    }
}
