using IssueTracker.Core.Interfaces;
using CoreModels = IssueTracker.Core.Models;
using IssueTracker.Infrastructure.Data;
using IssueTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Infrastructure.Repositories;

public class TicketsRepository(IssueTrackerDbContext dbContext) : ITicketsRepository
{
    public async Task<List<CoreModels.Ticket>> GetAll() => await dbContext.Tickets.Select(t=>t.ToCore()).ToListAsync();

    public async Task<CoreModels.Ticket?> GetById(int id) => (await dbContext.Tickets.FindAsync(id))?.ToCore();

    public async Task Add(CoreModels.Ticket ticket)
    {
        dbContext.Tickets.Add(ticket.ToDb());
        await dbContext.SaveChangesAsync();
    }

    public async Task Update(CoreModels.Ticket ticket)
    {
        dbContext.Tickets.Update(ticket.ToDb());
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var ticket = await dbContext.Tickets.FindAsync(id);
        if (ticket != null)
        {
            dbContext.Tickets.Remove(ticket);
            await dbContext.SaveChangesAsync();
        }
    }
}

public static class TicketsRepositoryExtensions
{
    public static CoreModels.Ticket ToCore(this Ticket ticket)
    {
        return new CoreModels.Ticket(
            Id: ticket.Id,
            Name: ticket.Name,
            Assignee: null,
            Reporter: null,
            Priority: (CoreModels.Priority)ticket.PriorityId,
            CreatedAt: ticket.CreatedAt,
            UpdatedAt: ticket.UpdatedAt);
    }

    public static Ticket ToDb(this CoreModels.Ticket ticket)
    {
        return new Ticket()
        {
            Id = ticket.Id,
            Name = ticket.Name,
            AssigneeId = ticket.Assignee?.Id,
            ReporterId = ticket.Reporter?.Id,
            PriorityId = (int)ticket.Priority,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }

}
    
