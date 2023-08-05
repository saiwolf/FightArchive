using System.ComponentModel.DataAnnotations;

namespace FightArchive.Data.Entities;

public class Contender : BaseEntity
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? ProfileUrl { get; set; }

    public List<Fight>? Fights { get; set; }
}
