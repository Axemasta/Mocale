using Mocale.Managers;

namespace Mocale.Extensions;

/// <summary>
/// Bindable Object Extension
/// </summary>
public static class BindableObjectExtension
{
    #region Void Methods

    /// <summary>
    /// Set Translation
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="translationKey">The translation key</param>
    /// <param name="converter">An optional converter</param>
    public static void SetTranslation(this BindableObject bindableObject, BindableProperty property, string translationKey, IValueConverter? converter = null)
    {
        ArgumentNullException.ThrowIfNull(bindableObject, nameof(bindableObject));

        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{translationKey}]",
            Source = MocaleLocator.TranslatorManager
        };

        if (converter is not null)
        {
            binding.Converter = converter;
        }

        bindableObject.SetBinding(property, binding);
    }

    /// <summary>
    /// Set Translation
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="source">The source binding</param>
    /// <param name="translationKey">The translation key</param>
    public static void SetTranslationBinding(this BindableObject bindableObject, BindableProperty property, Binding source, string translationKey)
    {
        ArgumentNullException.ThrowIfNull(bindableObject, nameof(bindableObject));
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        var binding = new MultiBinding()
        {
            Converter = new LocalizeBindingExtension(),
            Mode = BindingMode.OneWay,
            Bindings =
            [
                new Binding($"[{translationKey}]", BindingMode.OneWay, source: MocaleLocator.TranslatorManager),
                source,
            ]
        };

        bindableObject.SetBinding(property, binding);
    }

    /// <summary>
    /// Set Enum Translation
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="binding">The binding you wish to localize, the type must be an enum</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static void SetEnumTranslation(this BindableObject bindableObject, BindableProperty property, Binding binding, string stringFormat = "{0}")
    {
        ArgumentNullException.ThrowIfNull(bindableObject, nameof(bindableObject));

        var multiBinding = new MultiBinding()
        {
            StringFormat = stringFormat,
            Converter = new LocalizeEnumExtension(),
            Mode = BindingMode.OneWay,
            Bindings =
            [
                new Binding(nameof(TranslatorManager.CurrentCulture), BindingMode.OneWay, source: MocaleLocator.TranslatorManager),
                binding,
            ]
        };

        bindableObject.SetBinding(property, multiBinding);
    }

    #endregion

    #region Fluent Methods

    /// <summary>
    /// Set Translation
    /// </summary>
    /// <typeparam name="TView">The type of the view having the translation applied</typeparam>
    /// <param name="view">The view to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="translationKey">The translation key</param>
    /// <param name="converter">An optional converter</param>
    public static TView SetTranslation<TView>(this TView view, BindableProperty property, string translationKey, IValueConverter? converter = null)
        where TView : View
    {
        ArgumentNullException.ThrowIfNull(view, nameof(view));

        var binding = new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{translationKey}]",
            Source = MocaleLocator.TranslatorManager
        };

        if (converter is not null)
        {
            binding.Converter = converter;
        }

        view.SetBinding(property, binding);

        return view;
    }

    /// <summary>
    /// Set Translation
    /// </summary>
    /// <typeparam name="TView">The type of the view having the translation applied</typeparam>
    /// <param name="view">The view to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="source">The source binding</param>
    /// <param name="translationKey">The translation key</param>
    public static TView SetTranslationBinding<TView>(this TView view, BindableProperty property, Binding source, string translationKey)
        where TView : View
    {
        ArgumentNullException.ThrowIfNull(view, nameof(view));

        var binding = new MultiBinding()
        {
            Converter = new LocalizeBindingExtension(),
            Mode = BindingMode.OneWay,
            Bindings =
            [
                new Binding($"[{translationKey}]", BindingMode.OneWay, source: MocaleLocator.TranslatorManager),
                source,
            ]
        };

        view.SetBinding(property, binding);

        return view;
    }

    /// <summary>
    /// Set Enum Translation
    /// </summary>
    /// <typeparam name="TView">The type of the view having the translation applied</typeparam>
    /// <param name="view">The view to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="binding">The binding you wish to localize, the type must be an enum</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static TView SetEnumTranslation<TView>(this TView view, BindableProperty property, Binding binding, string stringFormat = "{0}")
    {
        ArgumentNullException.ThrowIfNull(view, nameof(view));

        if (view is not BindableObject bindableObject)
        {
            throw new NotSupportedException($"View {view.GetType().Name} must be a BindableObject to apply translation...");
        }

        var multiBinding = new MultiBinding()
        {
            StringFormat = stringFormat,
            Converter = new LocalizeEnumExtension(),
            Mode = BindingMode.OneWay,
            Bindings =
            [
                new Binding(nameof(TranslatorManager.CurrentCulture), BindingMode.OneWay, source: MocaleLocator.TranslatorManager),
                new Binding(binding.Path, binding.Mode, binding.Converter, binding.ConverterParameter, source: binding.Source),
            ]
        };

        bindableObject.SetBinding(property, multiBinding);

        return view;
    }

    #endregion
}
