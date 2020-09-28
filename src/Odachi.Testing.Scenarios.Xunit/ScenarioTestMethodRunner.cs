using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Odachi.Testing.Scenarios.Xunit
{
	/// <inheritdoc/>
	public class ScenarioTestMethodRunner : XunitTestMethodRunner
	{
		/// <inheritdoc/>
		public ScenarioTestMethodRunner(ITestMethod testMethod, IReflectionTypeInfo @class, IReflectionMethodInfo method, IEnumerable<IXunitTestCase> testCases, IMessageSink diagnosticMessageSink, IMessageBus messageBus, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource, object[] constructorArguments)
			: base(testMethod, @class, method, testCases, diagnosticMessageSink, messageBus, aggregator, cancellationTokenSource, constructorArguments)
		{
		}

		/// <inheritdoc/>
		protected override async Task<RunSummary> RunTestCaseAsync(IXunitTestCase testCase)
		{
			await using (Tracker.Create(ScenarioContext.CurrentCaseStore, new ScenarioScope<ITestCase>(testCase)))
			{
				return await base.RunTestCaseAsync(testCase);
			}
		}
	}
}
