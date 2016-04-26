function Exec
{
    # source: http://joshua.poehls.me/2012/powershell-script-module-boilerplate/

    [CmdletBinding()]
    param (
        [Parameter(Position=0, Mandatory=1)]
        [scriptblock]$Command,
        [Parameter(Position=1, Mandatory=0)]
        [string]$ErrorMessage = "Execution of command failed.`n$Command"
    )
    & $Command
    if ($LastExitCode -ne 0) {
        throw "Exec: $ErrorMessage"
    }
}

# source: https://github.com/enricosada/fsharp-dotnet-cli-samples

Write-Host "Installing .NET CLI.."

mkdir -Force ".\tools\" | Out-Null
Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0/scripts/obtain/install.ps1" -OutFile ".\tools\install.ps1"
$env:DOTNET_INSTALL_DIR = "$pwd\.dotnetcli"
Exec { .\tools\install.ps1 -Channel "preview" -version "$env:CLI_VERSION" -InstallDir "$env:DOTNET_INSTALL_DIR" -NoPath }
$env:Path = "$env:DOTNET_INSTALL_DIR;$env:Path"