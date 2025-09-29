using NeolantTestTask.Models;

namespace NeolantTestTask.Repositories;

public interface IPetsRepository
{
    Task<Animal?> GetByIdAsync(int id);
    Task<List<Animal>> GetByOwnerIdAsync(int ownerId);
    Task AddAsync(Animal data);
    Task UpdateAsync(Animal data);
    Task DeleteAsync(int id);
}

