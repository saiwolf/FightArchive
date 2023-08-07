using System.ComponentModel.DataAnnotations;

namespace FightArchive.Data.Entities;

public class Contender : BaseEntity
{
    [Required(ErrorMessage = "Please give your fighter a name!")]
    public string? Name { get; set; }
    [Required(ErrorMessage = "Please give us a link for more info about your fighter!")]
    [DataType(DataType.Url)]
    public string? ProfileUrl { get; set; }

    public List<Fight>? Fights { get; set; }

    public Guid AccountId { get; set; }
    public Account? Account { get; set; }
}
