using IssueTracker.Core.Interfaces;
using IssueTracker.Core.Models;

namespace IssueTracker.Core.Services;

public class TicketsService(ITicketsRepository ticketsRepository)
{
    public async Task<List<Ticket>> GetAll()
    {
        return await ticketsRepository.GetAll();
    }

    public async Task<Ticket?> GetById(int id)
    {
        return await ticketsRepository.GetById(id);
    }

    public async Task Add(Ticket ticket)
    {
        await ticketsRepository.Add(ticket);
    }

    public async Task Update(Ticket ticket)
    {
        await ticketsRepository.Update(ticket);
    }

    public async Task Delete(int id)
    {
        await ticketsRepository.Delete(id);
    }
}