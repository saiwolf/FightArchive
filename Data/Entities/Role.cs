namespace FightArchive.Data.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public List<Account> Accounts { get; set; } = new();
}
