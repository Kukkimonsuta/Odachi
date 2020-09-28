using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Odachi.Testing.Scenarios.Xunit
{
	/// <inheritdoc/>
	public class ScenarioTestCollectionRunner : XunitTestCollectionRunner
	{
		/// <inheritdoc/>
		public ScenarioTestCollectionRunner(ITestCollection testCollection, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ITestCaseOrderer testCaseOrderer, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
			: base(testCollection, testCases, diagnosticMessageSink, messageBus, testCaseOrderer, aggregator, cancellationTokenSource)
		{
		}

		/// <inheritdoc/>
		protected override async Task<RunSummary> RunTestClassAsync(ITestClass testClass, IReflectionTypeInfo @class, IEnumerable<IXunitTestCase> testCases)
		{
			await using (Tracker.Create(ScenarioContext.CurrentClassStore, new ScenarioScope<ITestClass>(testClass)))
			{
				return await new ScenarioTestClassRunner(testClass, @class, testCases, DiagnosticMessageSink, MessageBus, TestCaseOrderer, new ExceptionAggregator(Aggregator), CancellationTokenSource, CollectionFixtureMappings).RunAsync();
			}
		}
	}
}
