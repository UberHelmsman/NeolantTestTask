using System.ComponentModel.DataAnnotations;

namespace NeolantTestTask.Models;

public record User
{
    [Key] public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public List<Animal> Pets { get; set; } = new();

    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public string? AvatarUrl { get; set; }
}