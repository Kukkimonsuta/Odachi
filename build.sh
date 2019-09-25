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
_build "./src/Odachi.Abstractions" netstandard2.0
_build "./src/Odachi.Annotations" netstandard2.0
_build "./src/Odachi.AspNetCore.Authentication.Basic" netcoreapp3.0
_build "./src/Odachi.AspNetCore.JsonRpc" netcoreapp3.0
_build "./src/Odachi.AspNetCore.Mvc" netcoreapp3.0
_build "./src/Odachi.CodeGen" netstandard2.0
_build "./src/Odachi.CodeGen.CSharp" netstandard2.0
_build "./src/Odachi.CodeGen.TypeScript" netstandard2.0
_build "./src/Odachi.CodeGen.TypeScript.StackinoUno" netstandard2.0
_build "./src/Odachi.CodeGen.TypeScript.StackinoDue" netstandard2.0
_build "./src/Odachi.CodeModel" netstandard2.0
_build "./src/Odachi.CodeModel.Providers.FluentValidation" netstandard2.0
_build "./src/Odachi.CodeModel.Providers.JsonRpc" netstandard2.0
_build "./src/Odachi.CodeModel.Providers.Validation" netstandard2.0
_build "./src/Odachi.EntityFrameworkCore" netstandard2.0
_build "./src/Odachi.Extensions.Collections" netstandard2.0
_build "./src/Odachi.Extensions.Formatting" netstandard2.0
_build "./src/Odachi.Extensions.Primitives" netstandard2.0
_build "./src/Odachi.Extensions.Reflection" netstandard2.0
_build "./src/Odachi.Gettext" netstandard2.0
_build "./src/Odachi.JsonRpc.Client" netstandard2.0
_build "./src/Odachi.JsonRpc.Client.Http" netstandard2.0
_build "./src/Odachi.JsonRpc.Common" netstandard2.0
_build "./src/Odachi.JsonRpc.Server" netstandard2.0
_build "./src/Odachi.Mail" netstandard2.0
_build "./src/Odachi.RazorTemplating" netstandard2.0
_build "./src/Odachi.RazorTemplating.MSBuild" netstandard2.0
_build "./src/Odachi.Security" netstandard2.0
_build "./src/Odachi.Storage.Azure" netstandard2.0
_build "./src/Odachi.Storage.FileSystem" netstandard2.0
_build "./src/Odachi.Validation" netstandard2.0

echo
echo "Build samples.."
echo
_build "./samples/BasicAuthenticationSample" netcoreapp3.0
_build "./samples/JsonRpcSample" netcoreapp3.0
_build "./samples/JsonRpcClientSample" netcoreapp3.0
_build "./samples/MailSample" netcoreapp3.0

echo
echo "Build & run test.."
echo
_test "./test/Odachi.CodeGen.Tests" netcoreapp3.0
_test "./test/Odachi.CodeGen.TypeScript.Tests" netcoreapp3.0
_test "./test/Odachi.CodeGen.TypeScript.StackinoUno.Tests" netcoreapp3.0
_test "./test/Odachi.CodeModel.Tests" netcoreapp3.0
_test "./test/Odachi.Extensions.Formatting.Tests" netcoreapp3.0
_test "./test/Odachi.Extensions.Primitives.Tests" netcoreapp3.0
_test "./test/Odachi.Extensions.Reflection.Tests" netcoreapp3.0
_test "./test/Odachi.Gettext.Tests" netcoreapp3.0
_test "./test/Odachi.JsonRpc.Server.Tests" netcoreapp3.0
_test "./test/Odachi.RazorTemplating.Tests" netcoreapp3.0
_test "./test/Odachi.Security.Tests" netcoreapp3.0
_test "./test/Odachi.Storage.FileSystem.Tests" netcoreapp3.0
_test "./test/Odachi.Validation.Tests" netcoreapp3.0
