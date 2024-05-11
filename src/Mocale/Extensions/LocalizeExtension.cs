using Ardalis.GuardClauses;
namespace Mocale.Extensions;

[ContentProperty(nameof(Key))]
public class LocalizeExtension(ITranslatorManager translatorManager) : IMarkupExtension<BindingBase>
{
    private readonly ITranslatorManager translatorManager = Guard.Against.Null(translatorManager, nameof(translatorManager));

    public string? Key { get; set; }

    public IValueConverter? Converter { get; set; }

    public LocalizeExtension()
        : this(MocaleLocator.TranslatorManager)
    {
    }

    public BindingBase ProvideValue(IServiceProvider serviceProvider)
    {
        return new Binding
        {
            Mode = BindingMode.OneWay,
            Path = $"[{Key}]",
            Source = translatorManager,
            Converter = Converter,
        };
    }

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
    {
        return ProvideValue(serviceProvider);
    }
}
