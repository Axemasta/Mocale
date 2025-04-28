using System.Runtime.CompilerServices;

namespace Mocale.UnitTests;

public static class MocaleTestInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }
}
