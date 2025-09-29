using Microsoft.EntityFrameworkCore;
using NeolantTestTask.Data;
using NeolantTestTask.Models;

namespace NeolantTestTask.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Pets)
            .FirstOrDefaultAsync(u => u.Id == id);
    }


    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        var existingUser = await _context.Users.FindAsync(user.Id);
        if (existingUser != null)
        {
            existingUser.Username = user.Username;
            existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(User updatedUser, string? avatarFileName)
    {
        var existingUser = await _context.Users.FindAsync(updatedUser.Id);
        if (existingUser == null) return;

        existingUser.Name = updatedUser.Name;
        existingUser.Surname = updatedUser.Surname;
        existingUser.Email = updatedUser.Email;

        // если есть имя файла аватара, обновляем AvatarUrl
        if (!string.IsNullOrEmpty(avatarFileName))
            existingUser.AvatarUrl = "images/users/" + avatarFileName;

        await _context.SaveChangesAsync();
    }


    public async Task DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}