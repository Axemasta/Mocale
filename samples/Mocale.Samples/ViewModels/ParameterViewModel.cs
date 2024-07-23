namespace Mocale.Samples.ViewModels;
public sealed partial class ParameterViewModel : BaseViewModel
{
    [ObservableProperty]
    private string name;

    private static readonly Random random = new();

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
    }

    private string ChooseRandomName()
    {
        var index = random.Next(0, names.Length);

        var name = names[index];

        if (!name.Equals(Name, StringComparison.Ordinal))
        {
            return name;
        }

        return ChooseRandomName();
    }
}
