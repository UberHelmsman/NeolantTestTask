using Microsoft.EntityFrameworkCore;
using NeolantTestTask.Models;

namespace NeolantTestTask.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<User> Users { get; set; }
    public DbSet<DataSource> DataSources { get; set; }
    public DbSet<Animal> Pets { get; set; }
    public DbSet<Cat> Cats { get; set; }
    public DbSet<Dog> Dogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Animal>()
            .HasDiscriminator<string>("AnimalType")
            .HasValue<Cat>("Cat")
            .HasValue<Dog>("Dog");
        
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1, 
                Username = "admin", 
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), 
                Role = "Admin"
            }
        );

        modelBuilder.Entity<User>()
            .HasMany(u => u.Pets)
            .WithOne(p => p.Owner)
            .HasForeignKey(p => p.OwnerId)
            .IsRequired(true); 


        
        modelBuilder.Entity<DataSource>().HasData(
            new DataSource { Id = 1, Name = "Источник 1", IsActive = true },
            new DataSource { Id = 2, Name = "Источник 2", IsActive = false },
            new DataSource { Id = 3, Name = "Источник 3", IsActive = true }
        );
        
        modelBuilder.Entity<Cat>().HasData(
            new Cat { Id = 1, Name = "Kot", OwnerId = 1}
        );
        modelBuilder.Entity<Dog>().HasData(
            new Dog { Id = 2, Name = "Sobaka", OwnerId = 1 }
        );
    }
}