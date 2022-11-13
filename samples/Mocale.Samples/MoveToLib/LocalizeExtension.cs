namespace Mocale.Samples.MoveToLib
{
    [ContentProperty(nameof(Name))]
    public class LocalizeExtension : IMarkupExtension<BindingBase>
    {
        public string? Name { get; set; }

        public IValueConverter Converter { get; set; }

        public BindingBase ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding
            {
                Mode = BindingMode.OneWay,
                Path = $"[{Name}]",
                Source = LocalizationResourceManager.Instance,
                Converter = Converter,
            };
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }
    }
}
