#!/bin/bash

function _build {
    pushd $1
	dotnet restore
    dotnet build --configuration Release --framework $2
    popd
}

function _test {
    pushd $1
	dotnet restore
    dotnet test --configuration Release --framework $2
    popd
}

echo
echo "Display dotnet info.."
echo
dotnet --info

echo
echo "Build & pack libraries.."
echo
_build "./src/Odachi.Abstractions" netstandard1.3
_build "./src/Odachi.Annotations" netstandard1.3
_build "./src/Odachi.AspNetCore.Authentication.Basic" netstandard2.0
_build "./src/Odachi.AspNetCore.JsonRpc" netstandard1.5
_build "./src/Odachi.AspNetCore.Mvc" netstandard1.5
_build "./src/Odachi.AspNetCore.MvcPages" netstandard1.5
_build "./src/Odachi.CodeGen" netstandard1.3
_build "./src/Odachi.CodeGen.CSharp" netstandard1.6
_build "./src/Odachi.CodeGen.TypeScript" netstandard1.6
_build "./src/Odachi.CodeModel" netstandard1.5
_build "./src/Odachi.CodeModel.Providers.FluentValidation" netstandard1.5
_build "./src/Odachi.CodeModel.Providers.JsonRpc" netstandard1.5
_build "./src/Odachi.CodeModel.Providers.Validation" netstandard1.5
_build "./src/Odachi.EntityFrameworkCore" netstandard1.3
_build "./src/Odachi.Extensions.Collections" netstandard1.3
_build "./src/Odachi.Extensions.Formatting" netstandard1.5
_build "./src/Odachi.Extensions.Primitives" netstandard1.3
_build "./src/Odachi.Extensions.Reflection" netstandard1.3
_build "./src/Odachi.Gettext" netstandard1.3
_build "./src/Odachi.JsonRpc.Client" netstandard1.1
_build "./src/Odachi.JsonRpc.Client.Http" netstandard1.1
_build "./src/Odachi.JsonRpc.Common" netstandard1.1
_build "./src/Odachi.Localization" netstandard1.3
_build "./src/Odachi.Localization.Extraction" netstandard1.5
_build "./src/Odachi.Mail" netstandard1.3
_build "./src/Odachi.RazorTemplating" netstandard1.3
_build "./src/Odachi.RazorTemplating.MSBuild" netstandard1.3
_build "./src/Odachi.Security" netstandard1.3
_build "./src/Odachi.Validation" netstandard1.3

echo
echo "Build samples.."
echo
_build "./samples/BasicAuthenticationSample" netcoreapp2.0
_build "./samples/JsonRpcSample" netcoreapp1.1
_build "./samples/JsonRpcClientSample" netcoreapp1.1
_build "./samples/MailSample" netcoreapp1.1

echo
echo "Build & run test.."
echo
_test "./test/Odachi.AspNetCore.JsonRpc.Tests" netcoreapp1.1
_test "./test/Odachi.CodeGen.Tests" netcoreapp1.1
_test "./test/Odachi.Extensions.Formatting.Tests" netcoreapp1.1
_test "./test/Odachi.Extensions.Primitives.Tests" netcoreapp1.1
_test "./test/Odachi.Extensions.Reflection.Tests" netcoreapp1.1
_test "./test/Odachi.Gettext.Tests" netcoreapp1.1
_test "./test/Odachi.Localization.Extraction.Tests" netcoreapp1.1
_test "./test/Odachi.RazorTemplating.Tests" netcoreapp1.1
_test "./test/Odachi.Security.Tests" netcoreapp1.1
