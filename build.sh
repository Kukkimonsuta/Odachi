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
_build "./src/Odachi.AspNetCore.JsonRpc" netstandard2.0
_build "./src/Odachi.AspNetCore.Mvc" netstandard2.0
_build "./src/Odachi.AspNetCore.MvcPages" netstandard2.0
_build "./src/Odachi.CodeGen" netstandard1.5
_build "./src/Odachi.CodeGen.CSharp" netstandard1.6
_build "./src/Odachi.CodeGen.TypeScript" netstandard1.6
_build "./src/Odachi.CodeModel" netstandard1.5
_build "./src/Odachi.CodeModel.Providers.FluentValidation" netstandard1.5
_build "./src/Odachi.CodeModel.Providers.JsonRpc" netstandard2.0
_build "./src/Odachi.CodeModel.Providers.Validation" netstandard1.5
_build "./src/Odachi.EntityFrameworkCore" netstandard2.0
_build "./src/Odachi.Extensions.Collections" netstandard1.3
_build "./src/Odachi.Extensions.Formatting" netstandard1.5
_build "./src/Odachi.Extensions.Primitives" netstandard1.3
_build "./src/Odachi.Extensions.Reflection" netstandard1.3
_build "./src/Odachi.Gettext" netstandard1.3
_build "./src/Odachi.JsonRpc.Client" netstandard2.0
_build "./src/Odachi.JsonRpc.Client.Http" netstandard2.0
_build "./src/Odachi.JsonRpc.Common" netstandard2.0
_build "./src/Odachi.Mail" netstandard1.3
_build "./src/Odachi.RazorTemplating" netstandard2.0
_build "./src/Odachi.RazorTemplating.MSBuild" netstandard2.0
_build "./src/Odachi.Security" netstandard1.3
_build "./src/Odachi.Storage.Azure" netstandard2.0
_build "./src/Odachi.Storage.FileSystem" netstandard2.0
_build "./src/Odachi.Validation" netstandard1.0

echo
echo "Build samples.."
echo
_build "./samples/BasicAuthenticationSample" netcoreapp2.0
_build "./samples/JsonRpcSample" netcoreapp2.0
_build "./samples/JsonRpcClientSample" netcoreapp2.0
_build "./samples/MailSample" netcoreapp2.0

echo
echo "Build & run test.."
echo
_test "./test/Odachi.CodeGen.Tests" netcoreapp2.0
_test "./test/Odachi.CodeGen.TypeScript.Tests" netcoreapp2.0
_test "./test/Odachi.CodeModel.Tests" netcoreapp2.0
_test "./test/Odachi.Extensions.Formatting.Tests" netcoreapp2.0
_test "./test/Odachi.Extensions.Primitives.Tests" netcoreapp2.0
_test "./test/Odachi.Extensions.Reflection.Tests" netcoreapp2.0
_test "./test/Odachi.Gettext.Tests" netcoreapp2.0
_test "./test/Odachi.JsonRpc.Server.Tests" netcoreapp2.0
_test "./test/Odachi.RazorTemplating.Tests" netcoreapp2.0
_test "./test/Odachi.Security.Tests" netcoreapp2.0
_test "./test/Odachi.Storage.FileSystem.Tests" netcoreapp2.0
_test "./test/Odachi.Validation.Tests" netcoreapp2.0
