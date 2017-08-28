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
_build "./src/Odachi.Abstractions" netstandard13
_build "./src/Odachi.Annotations" netstandard13
_build "./src/Odachi.AspNetCore.Authentication.Basic" netstandard15
_build "./src/Odachi.AspNetCore.JsonRpc" netstandard15
_build "./src/Odachi.AspNetCore.Mvc" netstandard15
_build "./src/Odachi.AspNetCore.MvcPages" netstandard15
_build "./src/Odachi.CodeGen" netstandard13
_build "./src/Odachi.CodeGen.CSharp" netstandard16
_build "./src/Odachi.CodeGen.TypeScript" netstandard16
_build "./src/Odachi.CodeModel" netstandard15
_build "./src/Odachi.CodeModel.Providers.FluentValidation" netstandard15
_build "./src/Odachi.CodeModel.Providers.JsonRpc" netstandard15
_build "./src/Odachi.CodeModel.Providers.Validation" netstandard15
_build "./src/Odachi.EntityFrameworkCore" netstandard13
_build "./src/Odachi.Extensions.Collections" netstandard13
_build "./src/Odachi.Extensions.Formatting" netstandard15
_build "./src/Odachi.Extensions.Primitives" netstandard13
_build "./src/Odachi.Extensions.Reflection" netstandard13
_build "./src/Odachi.Gettext" netstandard13
_build "./src/Odachi.JsonRpc.Client" netstandard11
_build "./src/Odachi.JsonRpc.Client.Http" netstandard11
_build "./src/Odachi.JsonRpc.Common" netstandard11
_build "./src/Odachi.Localization" netstandard13
_build "./src/Odachi.Localization.Extraction" netstandard15
_build "./src/Odachi.Mail" netstandard13
_build "./src/Odachi.RazorTemplating" netstandard13
_build "./src/Odachi.RazorTemplating.MSBuild" netstandard13
_build "./src/Odachi.Security" netstandard13
_build "./src/Odachi.Validation" netstandard13

echo
echo "Build samples.."
echo
_build "./samples/BasicAuthenticationSample" netcoreapp1.1
_build "./samples/JsonRpcSample" netcoreapp1.1
_build "./samples/JsonRpcClientSample" netcoreapp1.1
_build "./samples/MailSample" netcoreapp1.1

echo
echo "Build & run test.."
echo
_test "./test/Odachi.AspNetCore.JsonRpc.Tests" netcoreapp11
_test "./test/Odachi.CodeGen.Tests" netcoreapp11
_test "./test/Odachi.Extensions.Formatting.Tests" netcoreapp11
_test "./test/Odachi.Extensions.Primitives.Tests" netcoreapp11
_test "./test/Odachi.Extensions.Reflection.Tests" netcoreapp11
_test "./test/Odachi.Gettext.Tests" netcoreapp11
_test "./test/Odachi.Localization.Extraction.Tests" netcoreapp11
_test "./test/Odachi.RazorTemplating.Tests" netcoreapp11
_test "./test/Odachi.Security.Tests" netcoreapp11
