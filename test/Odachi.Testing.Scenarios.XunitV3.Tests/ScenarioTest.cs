using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Odachi.Testing.Scenarios.XunitV3.Tests;

public partial class ScenarioTest : Scenario
{
    public class Counter
    {
        public int Value { get; set; }
    }

    public class SharedResource
    {
    }

    public class AsyncResource : IAsyncLifetime
    {
        public bool Initialized { get; private set; }

        public ValueTask InitializeAsync()
        {
            Initialized = true;

            return default;
        }

        public ValueTask DisposeAsync() => default;
    }

    private static int _sharedResourceCreated;

    private Fixture<Counter> CaseCounterFixture { get; } = Fixture.Case(() => new Counter());
    private Fixture<SharedResource> ClassResourceFixture { get; } = Fixture.Class(() =>
    {
        Interlocked.Increment(ref _sharedResourceCreated);

        return new SharedResource();
    });
    private Fixture<AsyncResource> CollectionResourceFixture { get; } = Fixture.Collection(() => new AsyncResource());

    [Fact]
    public void Fixtures_are_initialized_before_test()
    {
        Assert.NotNull(CaseCounter);
        Assert.NotNull(ClassResource);
        Assert.True(CollectionResource.Initialized);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Case_fixture_is_created_for_each_test(int _)
    {
        CaseCounter.Value++;

        Assert.Equal(1, CaseCounter.Value);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Class_fixture_is_shared_by_all_tests_of_class(int _)
    {
        Assert.NotNull(ClassResource);
        Assert.Equal(1, Volatile.Read(ref _sharedResourceCreated));
    }
}
