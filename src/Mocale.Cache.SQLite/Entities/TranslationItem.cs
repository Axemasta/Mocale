namespace Mocale.Cache.SQLite.Entities;

#nullable disable

[Table("Translations")]
internal class TranslationItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string CultureName { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }
}

#nullable enable
