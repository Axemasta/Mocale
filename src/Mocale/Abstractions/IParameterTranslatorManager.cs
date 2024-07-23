
namespace Mocale.Abstractions;

public interface IParameterTranslatorManager : ITranslatorManager
{
    void SetParameters(object[] parameters);
}
