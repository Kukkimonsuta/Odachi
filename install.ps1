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
Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/v1.0.0-preview2.1/scripts/obtain/dotnet-install.ps1" -OutFile ".\tools\install.ps1"
$env:DOTNET_INSTALL_DIR = "$pwd\.dotnetcli"
.\tools\install.ps1 -Channel "Preview" -Version "1.0.0-preview2-1-003177" -InstallDir "$env:DOTNET_INSTALL_DIR" -NoPath
$env:Path = "$env:DOTNET_INSTALL_DIR;$env:Path"