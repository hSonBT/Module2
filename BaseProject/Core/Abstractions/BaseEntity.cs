namespace BaseProject.Core.Abstractions;

public class BaseEntity
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public int EnterpriseId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}