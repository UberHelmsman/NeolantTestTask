using NeolantTestTask.Data;
using NeolantTestTask.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeolantTestTask.Repositories;

public class DataSourceRepository : IDataSourceRepository
{
    private readonly AppDbContext _context;

    public DataSourceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DataSource>> GetAllAsync()
    {
        return await _context.DataSources.ToListAsync();
    }

    public async Task<DataSource?> GetByIdAsync(int id)
    {
        return await _context.DataSources.FindAsync(id);
    }

    public async Task UpdateStatusAsync(int id, bool isActive)
    {
        var dataSource = await _context.DataSources.FindAsync(id);
        if (dataSource != null)
        {
            dataSource.IsActive = isActive;
            await _context.SaveChangesAsync();
        }
    }

    public async Task AddAsync(DataSource data)
    {
        _context.DataSources.Add(data);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DataSource data)
    {
        DataSource? existingData =  await _context.DataSources.FindAsync(data.Id);
        if (existingData != null)
        {
            existingData.Name = data.Name;
            existingData.IsActive = data.IsActive;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteAsync(int id)
    {
        DataSource? data = await _context.DataSources.FindAsync(id);
        if (data != null)
        {
            _context.DataSources.Remove(data);
            await _context.SaveChangesAsync();
        }
    }
}