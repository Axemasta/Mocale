namespace Mocale.UnitTests;

public abstract class FixtureBase<TSut>
{
    private Lazy<TSut> SutLazy { get; init; }

    protected TSut Sut => SutLazy.Value;

    public FixtureBase()
    {
        SutLazy = new Lazy<TSut>(CreateSystemUnderTest, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public abstract TSut CreateSystemUnderTest();
}

public abstract class FixtureBase
{
    private Lazy<object> SutLazy { get; init; }

    protected TSut GetSut<TSut>() => (TSut)SutLazy.Value;

    public FixtureBase()
    {
        SutLazy = new Lazy<object>(CreateSystemUnderTest, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public abstract object CreateSystemUnderTest();
}
