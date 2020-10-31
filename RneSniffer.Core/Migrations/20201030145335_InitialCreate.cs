using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RneSniffer.Core.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntrepriseRne",
                columns: table => new
                {
                    IdentifiantUnique = table.Column<string>(nullable: false),
                    DateCreation = table.Column<DateTime>(nullable: true),
                    DenominationSociale = table.Column<string>(nullable: true),
                    NomCommercial = table.Column<string>(nullable: true),
                    AdresseSiege = table.Column<string>(nullable: true),
                    LibelleActivite = table.Column<string>(nullable: true),
                    DenominationSocialeArabe = table.Column<string>(nullable: true),
                    NomCommercialArabe = table.Column<string>(nullable: true),
                    AdresseSiegeArabe = table.Column<string>(nullable: true),
                    LibelleActiviteArabe = table.Column<string>(nullable: true),
                    FormeJuridique = table.Column<string>(nullable: true),
                    FormeJuridiqueArabe = table.Column<string>(nullable: true),
                    EtatRegistre = table.Column<string>(nullable: true),
                    EtatRegistreArabe = table.Column<string>(nullable: true),
                    SituationFiscale = table.Column<string>(nullable: true),
                    SituationFiscaleArabe = table.Column<string>(nullable: true),
                    Region = table.Column<string>(nullable: true),
                    RegionArabe = table.Column<string>(nullable: true),
                    BureauRegional = table.Column<string>(nullable: true),
                    BureauRegionalArabe = table.Column<string>(nullable: true),
                    NomResponsable = table.Column<string>(nullable: true),
                    NomResponsableArabe = table.Column<string>(nullable: true),
                    PrenomResponsable = table.Column<string>(nullable: true),
                    PrenomResponsableArabe = table.Column<string>(nullable: true),
                    AnneeCreation = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntrepriseRne", x => x.IdentifiantUnique);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntrepriseRne");
        }
    }
}
