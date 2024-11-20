namespace Mocale.Extensions;

/// <summary>
/// Bindable Object Extension
/// </summary>
public static class BindableObjectExtension
{
    /// <summary>
    /// Set Translation
    /// </summary>
    /// <param name="bindableObject">The bindable object to apply a translation</param>
    /// <param name="property">The bindable property to target for translation</param>
    /// <param name="translationKey">The translation key</param>
    /// <param name="converter">An optional converter</param>
    public static void SetTranslation(this BindableObject bindableObject, BindableProperty property, string translationKey, IValueConverter? converter = null)
    {
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
}
