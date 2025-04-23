namespace Mocale.Samples.ViewModels;

public partial class ConverterViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial OrderStatus CurrentStatus { get; set; }

    public ObservableRangeCollection<OrderStatus> OrderStatuses { get; } =
    [
        new OrderStatus("Pending", 0),
        new OrderStatus("Shipped", 1),
        new OrderStatus("Delivered", 2),
        new OrderStatus("Cancelled", 3),
    ];

    public ConverterViewModel()
    {
        CurrentStatus = OrderStatuses[0];
    }
}

public partial class OrderStatus(string name, int stage) : ObservableObject
{
    [ObservableProperty]
    public partial string Name { get; set; } = name;

    [ObservableProperty]
    public partial int Stage { get; set; } = stage;
}
