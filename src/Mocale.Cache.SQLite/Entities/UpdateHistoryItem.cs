namespace Mocale.Cache.SQLite.Entities;

[Table("UpdateHistory")]
internal class UpdateHistoryItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string CultureName { get; set; } = string.Empty;

    public DateTime LastUpdated { get; set; }

    public string? ETag { get; set; }
}
