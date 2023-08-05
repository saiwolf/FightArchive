using System.ComponentModel.DataAnnotations;

namespace FightArchive.Data.Entities;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
    [Required]
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime? Updated { get; set; }
}
