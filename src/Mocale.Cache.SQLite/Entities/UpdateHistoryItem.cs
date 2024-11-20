namespace Mocale.Cache.SQLite.Entities;

#nullable disable

[Table("UpdateHistory")]
internal class UpdateHistoryItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string CultureName { get; set; }

    public DateTime LastUpdated { get; set; }
}

#nullable enable
