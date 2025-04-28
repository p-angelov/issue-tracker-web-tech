namespace IssueTracker.Core.Models;

public record Ticket(
    int Id,
    string Name,
    User Assignee,
    User Reporter,
    Priority Priority,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public enum Priority
{
    Low,
    Medium,
    High
}