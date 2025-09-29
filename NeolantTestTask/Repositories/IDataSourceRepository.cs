using NeolantTestTask.Models;

namespace NeolantTestTask.Repositories;

public interface IDataSourceRepository
{
    Task<IEnumerable<DataSource>> GetAllAsync();
    Task<DataSource?> GetByIdAsync(int id);
    Task UpdateStatusAsync(int id, bool isActive);
    
    Task AddAsync(DataSource data);
    
    Task UpdateAsync(DataSource data);
    Task DeleteAsync(int id);
}

