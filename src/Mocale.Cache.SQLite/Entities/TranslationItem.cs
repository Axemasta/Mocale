namespace Mocale.Cache.SQLite.Entities;

[Table("Translations")]
public class TranslationItem
{
    public int Id { get; set; }

    public required string CultureName { get; set; }

    public required string Key { get; set; }

    public string? Value { get; set; }
}
