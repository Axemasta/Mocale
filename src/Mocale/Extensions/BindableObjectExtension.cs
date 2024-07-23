namespace Mocale.Extensions;
public static class BindableObjectExtension
{
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
