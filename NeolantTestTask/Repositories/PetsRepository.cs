using Microsoft.EntityFrameworkCore;
using NeolantTestTask.Data;
using NeolantTestTask.Models;

namespace NeolantTestTask.Repositories;

public class PetsRepository : IPetsRepository
{
    private readonly AppDbContext _context;

    public PetsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Animal?> GetByIdAsync(int id)
    {
        return await _context.Pets
            .Include(p => p.Owner)  // загружаем владельца
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Animal>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.Pets
            .Where(p => p.OwnerId == ownerId)
            .Include(p => p.Owner)
            .ToListAsync();
    }

    public async Task AddAsync(Animal data)
    {
        _context.Pets.Add(data);
        await _context.SaveChangesAsync();
    }
    

    public async Task UpdateAsync(Animal data)
    {
        _context.Pets.Update(data);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var pet = await _context.Pets.FindAsync(id);
        if (pet != null)
        {
            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }
    }
}