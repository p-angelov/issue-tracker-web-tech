using IssueTracker.Core.Interfaces;
using CoreModels = IssueTracker.Core.Models;
using IssueTracker.Infrastructure.Data;
using IssueTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueTracker.Infrastructure.Repositories;

public class UsersRepository(IssueTrackerDbContext dbContext) : IUsersRepository
{
    public async Task<List<CoreModels.User>> GetAll() => 
        await dbContext.Users.Select(u => u.ToCore()).ToListAsync();

    public async Task<CoreModels.User?> GetById(int id) => 
        (await dbContext.Users.FindAsync(id))?.ToCore();
        
    public async Task<CoreModels.User?> GetByUsername(string username) => 
        (await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username))?.ToCore();
        
    public async Task<CoreModels.User?> GetByEmail(string email) => 
        (await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email))?.ToCore();

    public async Task<int> Add(CoreModels.User user)
    {
        User dbUser = user.ToDb();
        dbContext.Users.Add(dbUser);
        await dbContext.SaveChangesAsync();
        return dbUser.Id;
    }

    public async Task Update(CoreModels.User user)
    {
        dbContext.Users.Update(user.ToDb());
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var user = await dbContext.Users.FindAsync(id);
        if (user != null)
        {
            dbContext.Users.Remove(user);
            await dbContext.SaveChangesAsync();
        }
    }
}

public static class UsersRepositoryExtensions
{
    public static CoreModels.User ToCore(this User user) => 
        new()
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Email = user.Email,
            Role = user.Role
        };

    public static User ToDb(this CoreModels.User user) => 
        new()
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Email = user.Email,
            Role = user.Role
        };
} 