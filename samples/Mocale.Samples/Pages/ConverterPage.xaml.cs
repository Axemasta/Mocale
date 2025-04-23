using System.Globalization;
using System.Text;
using Mocale.Samples.ViewModels;
using Mocale.Translations;

namespace Mocale.Samples.Pages;

public partial class ConverterPage : BasePage<ConverterViewModel>
{
    public ConverterPage(ConverterViewModel viewModel)
        : base(viewModel)
    {
        InitializeComponent();
    }
}

public class SpongebobCaseConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || value is not string strValue)
        {
            return value;
        }

        var result = new StringBuilder();
        var toUpper = true;

        foreach (var c in strValue)
        {
            if (char.IsLetter(c))
            {
                result.Append(toUpper ? char.ToUpper(c, culture) : char.ToLower(c, culture));
                toUpper = !toUpper;
            }
            else
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class OrderStatusKeySelector : IKeyConverter
{
    public string? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || value is not OrderStatus orderStatus)
        {
            return null;
        }

        return orderStatus.Name switch
        {
            "Pending" => TranslationKeys.OrderStatusPending,
            "Shipped" => TranslationKeys.OrderStatusShipped,
            "Delivered" => TranslationKeys.OrderStatusDelivered,
            "Cancelled" => TranslationKeys.OrderStatusCancelled,
            _ => "Unknown order status."
        };
    }
}
