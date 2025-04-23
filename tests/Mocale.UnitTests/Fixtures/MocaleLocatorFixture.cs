namespace Mocale.UnitTests.Fixtures;

public class MocaleLocatorFixture : IDisposable
{
    public virtual void Dispose()
    {
        MocaleLocator.MocaleConfiguration = null;
        MocaleLocator.TranslatorManager = null;
        GC.SuppressFinalize(this);
    }
}

