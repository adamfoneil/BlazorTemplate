using System.ComponentModel.DataAnnotations;

namespace Domain.Conventions;

public abstract class BaseTable
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string? CreatedBy { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? ModifiedBy { get; set; }
    public DateTime? DateModified { get; set; }

    // todo: add other conventional properties here
}
