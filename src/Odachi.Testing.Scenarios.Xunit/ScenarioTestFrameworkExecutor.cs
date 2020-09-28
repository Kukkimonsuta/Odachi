using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Odachi.Testing.Scenarios.Xunit
{
	/// <inheritdoc/>
	public class ScenarioTestFrameworkExecutor : XunitTestFrameworkExecutor
	{
		/// <inheritdoc/>
		public ScenarioTestFrameworkExecutor(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
			: base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
		{
		}

		/// <inheritdoc/>
		protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
		{
			using (var assemblyRunner = new ScenarioTestAssemblyRunner(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions))
			await using (Tracker.Create(ScenarioContext.CurrentRunnerStore, new ScenarioScope<ScenarioTestAssemblyRunner>(assemblyRunner)))
			{
				await assemblyRunner.RunAsync();
			}
		}
	}
}
