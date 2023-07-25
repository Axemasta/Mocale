namespace Mocale.DAL.Entities;

[Table("UpdateHistory")]
public class UpdateHistoryItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public string CultureName { get; set; }

    public DateTime LastUpdated { get; set; }
}
