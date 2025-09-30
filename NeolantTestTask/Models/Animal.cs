using System.ComponentModel.DataAnnotations;

namespace NeolantTestTask.Models;

public abstract class Animal
{
    [Key] public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int OwnerId { get; set; }
    public required User Owner { get; set; }
}

public class Cat : Animal
{
}

public class Dog : Animal
{
}