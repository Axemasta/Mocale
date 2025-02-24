using System.ComponentModel;

namespace Mocale.Samples.Enums;

public enum Cities
{
    [Description("CityDescription_London")]
    [MocaleTranslationKey("CityDescription_London")]
    London,

    [Description("CityDescription_Nottingham")]
    [MocaleTranslationKey("CityDescription_Nottingham")]
    Nottingham,

    [Description("CityDescription_Newcastle")]
    [MocaleTranslationKey("CityDescription_Newcastle")]
    Newcastle,

    [Description("CityDescription_Manchester")]
    [MocaleTranslationKey("CityDescription_Manchester")]
    Manchester,
}

[AttributeUsage(AttributeTargets.Field)]
public class MocaleTranslationKeyAttribute(string key) : Attribute
{
    public string Key { get; set; } = key;
}
