namespace Mocale.UnitTests.TestHelpers;

public static class ServiceCollectionAssertions
{
    /// <summary>
    /// Asserts that a given service type is registered in the service collection.
    /// </summary>
    public static void ShouldContainService<TService>(this IServiceCollection services)
    {
        var service = services.FirstOrDefault(s => s.ServiceType == typeof(TService));
        Assert.NotNull(service);
    }

    /// <summary>
    /// Asserts that a given service type is NOT registered in the service collection.
    /// </summary>
    public static void ShouldNotContainService<TService>(this IServiceCollection services)
    {
        var service = services.FirstOrDefault(s => s.ServiceType == typeof(TService));
        Assert.Null(service);
    }

    /// <summary>
    /// Asserts that a specific implementation type is registered in the service collection.
    /// </summary>
    public static void ShouldContainImplementation<TService, TImplementation>(this IServiceCollection services)
    {
        var service = services.FirstOrDefault(s => s.ImplementationType == typeof(TImplementation));
        Assert.NotNull(service);
    }

    /// <summary>
    /// Asserts that a specific implementation type is NOT registered in the service collection.
    /// </summary>
    public static void ShouldNotContainImplementation<TService, TImplementation>(this IServiceCollection services)
    {
        var service = services.FirstOrDefault(s => s.ImplementationType == typeof(TImplementation));
        Assert.Null(service);
    }
}

