
namespace Mocale.Abstractions;

/// <summary>
/// Parameterized Translation Manager
/// </summary>
public interface IParameterTranslatorManager : ITranslatorManager
{
    /// <summary>
    /// Set the parameters to be used as part of translation
    /// </summary>
    /// <param name="parameters"></param>
    void SetParameters(object[] parameters);
}
