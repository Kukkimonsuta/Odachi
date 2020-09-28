using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Odachi.Testing.Scenarios.Xunit
{
	/// <inheritdoc/>
	public class ScenarioTestAssemblyRunner : XunitTestAssemblyRunner
	{
		/// <inheritdoc/>
		public ScenarioTestAssemblyRunner(ITestAssembly testAssembly, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
			: base(testAssembly, testCases, diagnosticMessageSink, executionMessageSink, executionOptions)
		{
		}

		/// <inheritdoc/>
		protected override async Task<RunSummary> RunTestCollectionAsync(IMessageBus messageBus, ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, CancellationTokenSource cancellationTokenSource)
		{
			await using (Tracker.Create(ScenarioContext.CurrentCollectionStore, new ScenarioScope<ITestCollection>(testCollection)))
			{
				return await new ScenarioTestCollectionRunner(testCollection, testCases, DiagnosticMessageSink, messageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), cancellationTokenSource).RunAsync();
			}
		}
	}
}
