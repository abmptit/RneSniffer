[CmdletBinding()]
PARAM(
    [parameter()]
    [ValidateSet('Initialize-Environment')]
    [String] 
    $function    
)

function Initialize-Environment {
     
    $location = Get-location
    $scriptLocation = $PSCommandPath | Split-Path -Parent

    try
    {
        Set-location $scriptLocation

        if (Test-Path RneSniffer\rne.db) {            
            rm RneSniffer\rne.db
        }
        
        New-Item RneSniffer\rne.db


        if (Test-Path .\RneSniffer.Core\Migrations) { 
            rm .\RneSniffer.Core\Migrations\*
        }

        dotnet tool install --global dotnet-ef
        
        dotnet build

        dotnet ef migrations add InitialCreate --context ApplicationDbContext -p .\RneSniffer.Core\RneSniffer.Core.csproj -s .\RneSniffer\RneSniffer.csproj
        
        dotnet ef database  update --context ApplicationDbContext -p .\RneSniffer.Core\RneSniffer.Core.csproj -s .\RneSniffer\RneSniffer.csproj --verbose
    } 
    catch
    {
        Set-location $scriptLocation
        throw;
    }

     Set-location $location
}

& $function