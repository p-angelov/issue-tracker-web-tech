using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Infrastructure.Entities;

public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int? AssigneeId { get; set; }
    [Required]
    public int? ReporterId { get; set; }

    [Required]
    public int PriorityId { get; set; }

    [Required]
    public DateTimeOffset CreatedAt { get; set; }

    [Required]
    public DateTimeOffset UpdatedAt { get; set; }
}