namespace FightArchive.Data.Entities.DTO;

public class AccountStorage
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
}
