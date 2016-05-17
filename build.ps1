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

function Build($path)
{
    Exec { pushd $path }
    Exec { dotnet build --configuration Release }
    Exec { popd }
}

function Pack($path)
{
    Exec { pushd $path }
    Exec { dotnet pack --output ../../build --configuration Release --version-suffix $buildNumber }
    Exec { popd }
}

function Test($path)
{
    Exec { pushd $path }
    Exec { dotnet test --configuration Release }
    Exec { popd }
}

Write-Host
Write-Host "Display dotnet info.."
Write-Host
Exec { dotnet --info }

Write-Host
Write-Host "Restore packages.."
Write-Host
Exec { dotnet restore }

Write-Host
Write-Host "Build & pack libraries.."
Write-Host
$buildNumber = $env:APPVEYOR_BUILD_NUMBER
if ([string]::IsNullOrEmpty($buildNumber)) {
    $buildNumber = "local"
}

Pack(".\src\Odachi.AspNetCore.Authentication.Basic")
Pack(".\src\Odachi.AspNetCore.Mvc")
Pack(".\src\Odachi.AspNetCore.MvcPages")
Pack(".\src\Odachi.Data")
Pack(".\src\Odachi.Gettext")
Pack(".\src\Odachi.Localization")
Pack(".\src\Odachi.Localization.Extraction")
Pack(".\src\Odachi.Localization.Extraction.Commands")
Pack(".\src\Odachi.Security")

Write-Host
Write-Host "Build samples.."
Write-Host
Build(".\samples\BasicAuthenticationSample");

Write-Host
Write-Host "Build & run test.."
Write-Host
Test(".\test\Odachi.Gettext.Tests");
Test(".\test\Odachi.Localization.Extraction.Tests");
Test(".\test\Odachi.Security.Tests");