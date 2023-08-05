using System.ComponentModel.DataAnnotations;

namespace FightArchive.Data.Entities;

public class Fight : BaseEntity
{
    [Required]
    public string? Name { get; set; }
    [Required]
    public string? LogUrl { get; set; }

    public string? Winner { get; set; }
    public List<Contender>? Contenders { get; set; }
}
