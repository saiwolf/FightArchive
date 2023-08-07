using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FightArchive.Data.Entities;

[Owned]
public class AccountToken
{
    [Key]
    public Guid Id { get; set; }

    public string Token { get; set; } = string.Empty;
    public string CreatedByIp { get; set; } = string.Empty;
    public string RevokedByIp { get; set; } = string.Empty;

    public DateTime Expires { get; set; }
    public DateTime? Revoked { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;

    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
        
    public bool IsExpired => DateTime.Now >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => Revoked is null && !IsExpired;
}
