namespace Mocale.UnitTests.Fixtures;

public class MocaleLocatorFixture : IDisposable
{
    public void Dispose()
    {
        MocaleLocator.MocaleConfiguration = null;
        MocaleLocator.TranslatorManager = null;
    }
}

