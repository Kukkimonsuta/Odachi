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
    Exec { dotnet msbuild "/t:Restore;Build" /p:VersionSuffix=$versionSuffix /p:Configuration=Release }
    Exec { popd }
}

function Pack($path)
{
    Exec { pushd $path }
    # Exec { dotnet pack --output ../../build --configuration Release --version-suffix $versionSuffix }
    # workaround for https://github.com/NuGet/Home/issues/4337
    Exec { dotnet msbuild "/t:Restore;Pack" /p:VersionSuffix=$versionSuffix /p:Configuration=Release /p:PackageOutputPath=../../build }
    Exec { popd }
}

function Test($path)
{
    Exec { pushd $path }
    Exec { dotnet test --configuration Release }
    Exec { popd }
}

$buildNumber = $env:APPVEYOR_BUILD_NUMBER
if (![string]::IsNullOrEmpty($buildNumber)) {
	$buildNumber = $buildNumber.PadLeft(6, "0")
}

$versionSuffix = ""
if ([string]::IsNullOrEmpty($buildNumber)) {
    $versionSuffix = "local"
}
elseif ([string]::Compare($env:APPVEYOR_REPO_BRANCH, "release", $True) -eq 0) {
	$versionSuffix = ""
}
else {
	$versionSuffix = "preview-$buildNumber"
}

$versionPrefix = ([xml](Get-Content .\tools\common.props)).Project.PropertyGroup | Where-Object {$_.VersionPrefix} | Select -ExpandProperty VersionPrefix
if ($env:APPVEYOR) {
    Update-AppveyorBuild -Version $versionPrefix-$versionSuffix
}

Write-Host
Write-Host "Building version $versionPrefix-$versionSuffix"
Write-Host

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
Pack(".\src\Odachi.Abstractions")
Pack(".\src\Odachi.Annotations")
Pack(".\src\Odachi.AspNetCore.Authentication.Basic")
Pack(".\src\Odachi.AspNetCore.Authentication.ApiKey")
Pack(".\src\Odachi.AspNetCore.JsonRpc")
Pack(".\src\Odachi.AspNetCore.Mvc")
Pack(".\src\Odachi.Build.OptionsFileGenerator")
Pack(".\src\Odachi.CodeGen")
Pack(".\src\Odachi.CodeGen.CSharp")
Pack(".\src\Odachi.CodeGen.TypeScript")
Pack(".\src\Odachi.CodeGen.TypeScript.StackinoUno")
Pack(".\src\Odachi.CodeGen.TypeScript.StackinoDue")
Pack(".\src\Odachi.CodeModel")
Pack(".\src\Odachi.CodeModel.Providers.FluentValidation")
Pack(".\src\Odachi.CodeModel.Providers.JsonRpc")
Pack(".\src\Odachi.CodeModel.Providers.Validation")
Pack(".\src\Odachi.EntityFrameworkCore")
Pack(".\src\Odachi.Extensions.Collections")
Pack(".\src\Odachi.Extensions.Formatting")
Pack(".\src\Odachi.Extensions.Primitives")
Pack(".\src\Odachi.Extensions.Reflection")
Pack(".\src\Odachi.Gettext")
Pack(".\src\Odachi.JsonRpc.Client")
Pack(".\src\Odachi.JsonRpc.Client.Http")
Pack(".\src\Odachi.JsonRpc.Common")
Pack(".\src\Odachi.JsonRpc.Server")
Pack(".\src\Odachi.Security")
Pack(".\src\Odachi.Storage.Azure")
Pack(".\src\Odachi.Storage.FileSystem")
Pack(".\src\Odachi.Testing.Scenarios")
Pack(".\src\Odachi.Testing.Scenarios.Data")
Pack(".\src\Odachi.Testing.Scenarios.Data.MySql")
Pack(".\src\Odachi.Testing.Scenarios.Data.Oracle")
Pack(".\src\Odachi.Testing.Scenarios.Data.PostgreSQL")
Pack(".\src\Odachi.Testing.Scenarios.Data.SqlServer")
Pack(".\src\Odachi.Testing.Scenarios.Xunit")
Pack(".\src\Odachi.Validation")

Write-Host
Write-Host "Build samples.."
Write-Host
Build(".\samples\BasicAuthenticationSample");
Build(".\samples\JsonRpcSample");
Build(".\samples\JsonRpcClientSample");

Write-Host
Write-Host "Build & run test.."
Write-Host
Test(".\test\Odachi.Build.OptionsFileGenerator.Tests");
Test(".\test\Odachi.CodeGen.Tests");
Test(".\test\Odachi.CodeGen.TypeScript.Tests");
Test(".\test\Odachi.CodeGen.TypeScript.StackinoUno.Tests");
Test(".\test\Odachi.CodeGen.TypeScript.StackinoDue.Tests");
Test(".\test\Odachi.CodeModel.Tests");
Test(".\test\Odachi.CodeModel.Providers.FluentValidation.Tests");
Test(".\test\Odachi.CodeModel.Providers.JsonRpc.Tests");
Test(".\test\Odachi.Extensions.Formatting.Tests");
Test(".\test\Odachi.Extensions.Primitives.Tests");
Test(".\test\Odachi.Extensions.Reflection.Tests");
Test(".\test\Odachi.Gettext.Tests");
Test(".\test\Odachi.JsonRpc.Server.Tests");
Test(".\test\Odachi.Security.Tests");
Test(".\test\Odachi.Storage.FileSystem.Tests");
Test(".\test\Odachi.Validation.Tests");
