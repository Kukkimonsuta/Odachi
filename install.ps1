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

function GetRequiredVersion()
{    
    $captures = gc "global.json" | 
                select-string -Pattern '"version"\s*:\s*"(?<version>[0-9a-zA-Z\-\.]+)"'
                 
    If ($captures.Matches.Count -le 0)
    {
        Throw "Cannot resolve required version"
    }

    $captures.Matches[0].Groups["version"].Value
}

$dnvmInstalled = Get-Command dnvm -erroraction 'silentlycontinue'
If (!$dnvmInstalled)
{
    "Installing DNVM..."
    ""
    &{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}
    ""
}

$version = GetRequiredVersion;

"Installing runtimes $version..."
""
Exec { dnvm install $version -u -r clr }
Exec { dnvm install $version -u -r coreclr -alias default }
""