namespace Mocale.UnitTests.Fixtures;

public abstract class FixtureBase<TSut> : MocaleLocatorFixture
{
    private Lazy<TSut> SutLazy { get; init; }

    protected TSut Sut => SutLazy.Value;

    public FixtureBase()
    {
        SutLazy = new Lazy<TSut>(CreateSystemUnderTest, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public abstract TSut CreateSystemUnderTest();
}

public abstract class FixtureBase : MocaleLocatorFixture
{
    private Lazy<object> SutLazy { get; init; }

    protected TSut GetSut<TSut>() => (TSut)SutLazy.Value;

    public FixtureBase()
    {
        SutLazy = new Lazy<object>(CreateSystemUnderTest, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public abstract object CreateSystemUnderTest();
}
