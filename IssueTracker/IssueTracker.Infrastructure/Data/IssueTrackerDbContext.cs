using IssueTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Infrastructure.Data;

public class IssueTrackerDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Ticket> Tickets => Set<Ticket>();
}