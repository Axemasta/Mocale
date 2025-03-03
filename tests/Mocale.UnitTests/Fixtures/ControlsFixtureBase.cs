using System.Reflection;

namespace Mocale.UnitTests.Fixtures;

public class ControlsFixtureBase
{
    public Application TestApplication { get; }

    public ControlsFixtureBase()
    {
        var app = new Application();
        SetDispatcher(app);
        TestApplication = app;
    }

    private void SetDispatcher(BindableObject target)
    {
        var bindableObjectType = typeof(BindableObject);

        var dispatcherField = bindableObjectType.GetField("_dispatcher", BindingFlags.Instance | BindingFlags.NonPublic);

        if (dispatcherField is null)
        {
            throw new InvalidOperationException("Could not find _dispatcher field on BindableObject");
        }

        dispatcherField.SetValue(target, new DispatcherStub());
    }
}

public class DispatcherStub : IDispatcher
{
    public bool Dispatch(Action action)
    {
        action.Invoke();
        return true;
    }

    public bool DispatchDelayed(TimeSpan delay, Action action)
    {
        action.Invoke();
        return true;
    }

    public IDispatcherTimer CreateTimer()
    {
        return Mock.Of<IDispatcherTimer>();
    }

    public bool IsDispatchRequired { get; }
}
