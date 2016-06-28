#!/bin/bash

function _build {
    pushd $1
    dotnet build --configuration Release --framework $2
    popd
}

function _test {
    pushd $1
    dotnet test --configuration Release --framework $2
    popd
}

echo
echo "Display dotnet info.."
echo
dotnet --info

echo
echo "Restore packages.."
echo
dotnet restore

echo
echo "Build & pack libraries.."
echo

_build "./src/Odachi.AspNetCore.Authentication.Basic" netstandard15
_build "./src/Odachi.AspNetCore.Mvc" netstandard15
_build "./src/Odachi.AspNetCore.MvcPages" netstandard15
_build "./src/Odachi.Data" netstandard13
_build "./src/Odachi.Gettext" netstandard13
_build "./src/Odachi.Localization" netstandard13
_build "./src/Odachi.Localization.Extraction" netstandard15
_build "./src/Odachi.Security" netstandard13

echo
echo "Build samples.."
echo
_build "./samples/BasicAuthenticationSample" netcoreapp10

echo
echo "Build & run test.."
echo
_test "./test/Odachi.Gettext.Tests" netcoreapp10
_test "./test/Odachi.Localization.Extraction.Tests" netcoreapp10
_test "./test/Odachi.Security.Tests" netcoreapp10