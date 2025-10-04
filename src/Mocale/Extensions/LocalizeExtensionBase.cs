using System.ComponentModel;
using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
/// Localize Extension Base
/// </summary>
public abstract class LocalizeExtensionBase : BindableObject, IDisposable
{
    // ReSharper disable once InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
    internal readonly ITranslatorManager translatorManager;
#pragma warning restore IDE1006 // Naming Styles

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ITranslatorManager GetTranslatorManager()
    {
        return translatorManager;
    }

    /// <summary>
    /// Localize Extension Base Constructor
    /// </summary>
    /// <param name="translatorManager">The translator manager instance to bind to</param>
    protected LocalizeExtensionBase(ITranslatorManager translatorManager)
    {
        this.translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

        this.translatorManager.PropertyChanged += OnTranslatorManagerPropertyChanged;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnTranslatorManagerPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {

    }

    /// <inheritdoc />
    public void Dispose()
    {
        this.translatorManager.PropertyChanged -= OnTranslatorManagerPropertyChanged;
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Base class for localize extensions
/// </summary>
/// <param name="translatorManager"></param>
public abstract class LocalizeBindingExtensionBase(ITranslatorManager translatorManager)
    : LocalizeExtensionBase(translatorManager), IMarkupExtension<BindingBase>
{
    /// <inheritdoc />
    public abstract BindingBase ProvideValue(IServiceProvider serviceProvider);

    /// <inheritdoc />
    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}

/// <summary>
/// Base class for localize extensions that use MultiBindings
/// </summary>
/// <param name="translatorManager"></param>
public abstract class LocalizeMultiBindingExtensionBase(ITranslatorManager translatorManager)
    : LocalizeExtensionBase(translatorManager), IMarkupExtension<MultiBinding>
{
    /// <inheritdoc />
    public abstract MultiBinding ProvideValue(IServiceProvider serviceProvider);

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}

