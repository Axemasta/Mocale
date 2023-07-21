namespace Mocale.Extensions;

[ContentProperty(nameof(Key))]
public class LocalizeExtension : IMarkupExtension<BindingBase>
{
    private readonly ILocalizationManager localizationManager;

    public string? Key { get; set; }

    public IValueConverter? Converter { get; set; }

    public LocalizeExtension()
        : this(MocaleLocator.LocalizationManager)
    {
    }

    public LocalizeExtension(ILocalizationManager localizationManager)
    {
        this.localizationManager = localizationManager;
    }

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Key}]",
            Source = localizationManager,
            Converter = Converter,
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
