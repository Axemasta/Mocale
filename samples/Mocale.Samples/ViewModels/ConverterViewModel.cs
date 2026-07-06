namespace Mocale.Samples.ViewModels;

public partial class ConverterViewModel : BaseViewModel
{
	[ObservableProperty]
	public partial OrderStatus CurrentStatus { get; set; }

	public ObservableRangeCollection<OrderStatus> OrderStatuses { get; } =
	[
		new("Pending", 0),
		new("Shipped", 1),
		new("Delivered", 2),
		new("Cancelled", 3),
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
