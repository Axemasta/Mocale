using System.ComponentModel;
using Ardalis.GuardClauses;

namespace Mocale.Extensions;

/// <summary>
/// Localize Extension Base
/// </summary>
/// <param name="translatorManager">The translator manager instance to bind to</param>
public abstract class LocalizeExtensionBase(ITranslatorManager translatorManager)
{
    // ReSharper disable once InconsistentNaming
#pragma warning disable IDE1006 // Naming Styles
    internal readonly ITranslatorManager translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));
#pragma warning restore IDE1006 // Naming Styles

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal ITranslatorManager GetTranslatorManager()
    {
        return translatorManager;
    }
}

/// <summary>
/// Base class for localize extensions
/// </summary>
/// <param name="translatorManager"></param>
public abstract class LocalizeBindingExtensionBase(ITranslatorManager translatorManager)
    : LocalizeExtensionBase(translatorManager), IMarkupExtension<Binding>
{
    /// <inheritdoc />
    public abstract Binding ProvideValue(IServiceProvider serviceProvider);

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

