using System;
using System.ComponentModel.DataAnnotations;

namespace RneSniffer.Core
{
    public class EntrepriseRne
    {
        [Key]
        public string IdentifiantUnique { get; set; }

        public DateTime? DateCreation { get; set; }

        public string DenominationSociale { get; set; }

        public string NomCommercial { get; set; }

        public string AdresseSiege { get; set; }

        public string LibelleActivite { get; set; }

        public string DenominationSocialeArabe { get; set; }

        public string NomCommercialArabe { get; set; }

        public string AdresseSiegeArabe { get; set; }

        public string LibelleActiviteArabe { get; set; }

        public string FormeJuridique { get; set; }
        public string FormeJuridiqueArabe { get; set; }

        public string EtatRegistre { get; set; }
        public string EtatRegistreArabe { get; set; }

        public string SituationFiscale { get; set; }

        public string SituationFiscaleArabe { get; set; }

        public string Region { get; set; }

        public string RegionArabe { get; set; }

        public string BureauRegional { get; set; }

        public string BureauRegionalArabe { get; set; }

        public string NomResponsable { get; set; }

        public string NomResponsableArabe { get; set; }

        public string PrenomResponsable { get; set; }

        public string PrenomResponsableArabe { get; set; }

        public string AnneeCreation { get; set; }

    }
}