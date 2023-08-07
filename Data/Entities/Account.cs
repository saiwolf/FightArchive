namespace FightArchive.Data.Entities;

public class Account : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash {  get; set; } = string.Empty;

    public Guid RoleId { get; set; }
    public Role Role { get; set; } = new();
    public List<Contender> Characters { get; set; } = new();
    public List<AccountToken> AccountTokens { get; set; } = new();

    public bool OwnsToken(string token) =>
        AccountTokens?.Find(x => x.Token == token) != null;
}
