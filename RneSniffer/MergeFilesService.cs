using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Portal.Core.Data;
using RneSniffer.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RneSniffer
{
    public class MergeFileService
       : ServiceBase, IDisposable
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<MergeFileService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly CancellationTokenRegistration _applicationStoppingRegistration;
        private readonly CancellationTokenRegistration _applicationStoppedRegistration;

        public MergeFileService(
              IHostApplicationLifetime applicationLifetime,
              ApplicationDbContext context,
              ILogger<MergeFileService> logger)
              : base(applicationLifetime, logger)
        {
            _logger = logger;
            _context = context;
            _applicationLifetime = applicationLifetime;
            _applicationStoppingRegistration = _applicationLifetime.ApplicationStopping.Register(ArreterService);
            _applicationStoppedRegistration = _applicationLifetime.ApplicationStopped.Register(FinService);
        }

        public override async Task ExecuterTraitementAsync(CancellationToken stoppingToken)
        {
            try
            {
                await Task.Delay(100);
                MergeDataInOneFile("2019");
            }
            catch (Exception ex)
            {

            }
        }

        private void ArreterService()
        {
            _logger.LogInformation("Le service est en cours d'arrêt...");
        }

        private void FinService()
        {
            _logger.LogInformation("Service terminé");
        }

        private void MergeDataInOneFile(string annee)
        {
            var entrepriseBases = _context.EntrepriseRne.ToList();

            var files = Directory.GetFiles($"backup\\rne\\{annee}", "*.json");
            var listeEntreprises = new List<EntrepriseRne>();
            foreach (var file in files)
            {
                var jsonFile = File.ReadAllText(file);
                var entreprises = JsonConvert.DeserializeObject<List<EntrepriseRne>>(jsonFile);
                foreach (var entreprise in entreprises)
                {
                    entreprise.AnneeCreation = annee;
                    if (!listeEntreprises.Contains(entreprise))
                    {
                        listeEntreprises.Add(entreprise);
                    }
                }
            }

           
            foreach (var entreprise in listeEntreprises)
            {
                var entrepriseBase = entrepriseBases?.FirstOrDefault(e => e.IdentifiantUnique == entreprise.IdentifiantUnique);
                if (entrepriseBase != null)
                {
                    _context.Update(entreprise);
                }
                else
                {
                    _context.Add(entreprise);
                };
            }
        }
    }
}