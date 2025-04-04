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

        var extension = new LocalizeExtension
        {
            Key = translationKey,
            Converter = converter
        };

        var binding = extension.ProvideValue(EmptyServiceProvider.Instance);

        bindableObject.SetBinding(property, binding);
    }

    /// <summary>
    /// Set Translation With Single Binding
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="source">The source binding</param>
    /// <param name="translationKey">The translation key</param>
    /// <param name="stringFormat">The string format to pass to the binding</param>
    public static void SetTranslationBinding(this BindableObject bindableObject, BindableProperty property, Binding source, string translationKey, string stringFormat = "{0}")
    {
        ArgumentNullException.ThrowIfNull(bindableObject, nameof(bindableObject));
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        var extension = new LocalizeBindingExtension
        {
            TranslationKey = translationKey,
            Path = source.Path,
            Source = source.Source,
            Mode = source.Mode,
            StringFormat = stringFormat,
            Converter = source.Converter,
            ConverterParameter = source.ConverterParameter,
        };

        var binding = extension.ProvideValue(EmptyServiceProvider.Instance);

        bindableObject.SetBinding(property, binding);
    }

    /// <summary>
    /// Set Enum Translation
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="source">The binding you wish to localize, the type must be an enum</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static void SetEnumTranslation(this BindableObject bindableObject, BindableProperty property, Binding source, string stringFormat = "{0}")
    {
        ArgumentNullException.ThrowIfNull(bindableObject, nameof(bindableObject));
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        var extension = new LocalizeEnumExtension()
        {
            Path = source.Path,
            Source = source.Source,
            Mode = source.Mode,
            StringFormat = stringFormat,
            Converter = source.Converter,
            ConverterParameter = source.ConverterParameter,
        };

        bindableObject.SetBinding(property, extension.ProvideValue(EmptyServiceProvider.Instance));
    }

    /// <summary>
    /// Set Enum Value Translation
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="enumValue">The enum value to localize</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static void SetEnumValueTranslation(this BindableObject bindableObject, BindableProperty property, Enum enumValue, string stringFormat = "{0}")
    {
        ArgumentNullException.ThrowIfNull(bindableObject, nameof(bindableObject));

        var extension = new LocalizeEnumExtension()
        {
            Source = enumValue,
            Mode = BindingMode.OneWay,
            StringFormat = stringFormat,
        };

        bindableObject.SetBinding(property, extension.ProvideValue(EmptyServiceProvider.Instance));
    }

    /// <summary>
    /// Set Translation With Multiple Bindings
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="translationKey"></param>
    /// <param name="sources">The bindings you wish to use as parameters to localize</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static void SetTranslationMultiBinding(this BindableObject bindableObject, BindableProperty property, string translationKey, Binding[] sources, string stringFormat = "{0}")
    {
        ArgumentNullException.ThrowIfNull(bindableObject, nameof(bindableObject));
        ArgumentNullException.ThrowIfNull(sources, nameof(sources));

        var extension = new LocalizeMultiBindingExtension()
        {
            TranslationKey = translationKey,
            StringFormat = stringFormat,
        };

        foreach (var source in sources)
        {
            extension.Bindings.Add(source);
        }

        var binding = extension.ProvideValue(EmptyServiceProvider.Instance);

        bindableObject.SetBinding(property, binding);
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
        SetTranslation(view as BindableObject, property, translationKey, converter);
        return view;
    }

    /// <summary>
    /// Set Translation With Single Binding
    /// </summary>
    /// <typeparam name="TView">The type of the view having the translation applied</typeparam>
    /// <param name="view">The view to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="source">The source binding</param>
    /// <param name="translationKey">The translation key</param>
    public static TView SetTranslationBinding<TView>(this TView view, BindableProperty property, Binding source, string translationKey)
        where TView : View
    {
        SetTranslationBinding(view as BindableObject, property, source, translationKey);
        return view;
    }

    /// <summary>
    /// Set Enum Translation
    /// </summary>
    /// <typeparam name="TView">The type of the view having the translation applied</typeparam>
    /// <param name="view">The view to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="source">The binding you wish to localize, the type must be an enum</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static TView SetEnumTranslation<TView>(this TView view, BindableProperty property, Binding source, string stringFormat = "{0}")
        where TView : View
    {
        SetEnumTranslation(view as BindableObject, property, source, stringFormat);
        return view;
    }

    /// <summary>
    /// Set Enum Value Translation
    /// </summary>
    /// <typeparam name="TView">The type of the view having the translation applied</typeparam>
    /// <param name="view">The view to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="enumValue">The enum value to localize</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static TView SetEnumValueTranslation<TView>(this TView view, BindableProperty property, Enum enumValue, string stringFormat = "{0}")
        where TView : View
    {
        SetEnumValueTranslation(view as BindableObject, property, enumValue, stringFormat);
        return view;
    }

    /// <summary>
    /// Set Translation With Multiple Bindings
    /// </summary>
    /// <typeparam name="TView">The type of the view having the translation applied</typeparam>
    /// <param name="view">The view to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="translationKey"></param>
    /// <param name="sources">The bindings you wish to use as parameters to localize</param>
    /// <param name="stringFormat">The string format to apply to the binding</param>
    public static TView SetTranslationMultiBinding<TView>(this TView view, BindableProperty property, string translationKey, Binding[] sources, string stringFormat = "{0}")
        where TView : View
    {
        SetTranslationMultiBinding(view as BindableObject, property, translationKey, sources, stringFormat);
        return view;
    }

    #endregion
}

internal class EmptyServiceProvider : IServiceProvider
{
    public object? GetService(Type serviceType)
    {
        return null;
    }

    private static Lazy<IServiceProvider> EmptyLazy => new(() => new EmptyServiceProvider(), LazyThreadSafetyMode.ExecutionAndPublication);

    internal static IServiceProvider Instance => EmptyLazy.Value;
}
