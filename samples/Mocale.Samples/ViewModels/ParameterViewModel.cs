namespace Mocale.Samples.ViewModels;

public sealed partial class ParameterViewModel : BaseViewModel
{
    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial DateTime Date { get; set; }

    [ObservableProperty]
    public partial int Number { get; set; }

    private static readonly Random Random = new();

    private readonly string[] names =
        [
            "Ivory",
            "Abdul",
            "Maurise",
            "Jared",
            "Gilles",
            "Andrew",
            "Bernadina",
            "Ron",
            "Joshua",
            "Silas",
            "Marc"
        ];


    public ParameterViewModel()
    {
        Name = ChooseRandomName();
        Date = new DateTime(2015, 11, 14);
        Number = Random.Next(1, 1000);
    }

    private string ChooseRandomName()
    {
        var index = Random.Next(0, names.Length);

        var name = names[index];

        if (!name.Equals(Name, StringComparison.Ordinal))
        {
            return name;
        }

        return ChooseRandomName();
    }
}
