using IssueTracker.Core.Models;

namespace IssueTracker.Core.Interfaces;

public interface ITicketsRepository
{
    Task<List<Ticket>> GetAll();

    Task<List<Ticket>> GetById(int id);

    Task Add(Ticket ticket);

    Task Update(Ticket ticket);

    Task Delete(int id);

}