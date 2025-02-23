namespace Mocale.Abstractions;

internal interface IInternalTranslatorManager : ITranslatorManager, ITranslationUpdater
{
    void RaisePropertyChanged(string? propertyName = null);
}
