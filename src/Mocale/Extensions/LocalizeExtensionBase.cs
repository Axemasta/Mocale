using System.ComponentModel;
using Ardalis.GuardClauses;

namespace Mocale.Extensions;

public abstract class LocalizeExtensionBase(ITranslatorManager translatorManager)
{
    // ReSharper disable once InconsistentNaming
    internal readonly ITranslatorManager translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

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

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}

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

