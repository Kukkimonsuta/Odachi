#!/bin/bash

function _build
{
    pushd $1
    dotnet build --configuration Release
    popd
}

function _pack
    pushd $1
    dotnet pack --output ../../build --configuration Release --version-suffix $buildNumber
    popd
}

function _test {
    pushd $1
    dotnet test --configuration Release
    popd
}

echo
echo "Display dotnet info.."
echo
dotnet --info

echo
echo "Restore packages.."
echo
Exec { dotnet restore }

echo
echo "Build & pack libraries.."
echo

BUILD_NUMBER = $env:APPVEYOR_BUILD_NUMBER
if [ -z "$BUILD_NUMBER" ]; then
    BUILD_NUMBER=local
fi

_pack ".\src\Odachi.AspNetCore.Authentication.Basic"
_pack ".\src\Odachi.AspNetCore.Mvc"
_pack ".\src\Odachi.AspNetCore.MvcPages"
_pack ".\src\Odachi.Data"
_pack ".\src\Odachi.Gettext"
_pack ".\src\Odachi.Localization"
_pack ".\src\Odachi.Localization.Extraction"
_pack ".\src\Odachi.Localization.Extraction.Commands"
_pack ".\src\Odachi.Security"

echo
echo "Build samples.."
echo
_build ".\samples\BasicAuthenticationSample"

echo
echo "Build & run test.."
echo
_test ".\test\Odachi.Gettext.Tests"
_test ".\test\Odachi.Localization.Extraction.Tests"