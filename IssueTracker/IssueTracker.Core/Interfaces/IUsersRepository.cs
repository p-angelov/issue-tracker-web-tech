using IssueTracker.Core.Models;

namespace IssueTracker.Core.Interfaces;

public interface IUsersRepository
{
    Task<List<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User?> GetByUsername(string username);
    Task<User?> GetByEmail(string email);
    Task<int> Add(User user);
    Task Update(User user);
    Task Delete(int id);
} 