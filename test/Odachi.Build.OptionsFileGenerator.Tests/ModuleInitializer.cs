using System.Runtime.CompilerServices;
using Odachi.Build.OptionsFileGenerator.Tests.Framework;
using VerifyTests;

namespace Odachi.Build.OptionsFileGenerator.Tests;

public static class ModuleInitializer
{
	[ModuleInitializer]
	public static void Init()
	{
		VerifySourceGenerators.Initialize();
		VerifyOptionsFileGenerator.Initialize();
	}
}
