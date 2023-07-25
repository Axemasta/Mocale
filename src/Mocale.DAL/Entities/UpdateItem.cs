namespace Mocale.DAL.Entities;

[Table("UpdateHistory")]
public class UpdateItem
{
    [PrimaryKey]
    [AutoIncrement]
    public int Id { get; set; }

    public required string CultureName { get; set; }

    public DateTime? LastUpdated { get; set; }
}
