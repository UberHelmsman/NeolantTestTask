using System.ComponentModel.DataAnnotations;

namespace NeolantTestTask.Models;

public class DataSource
{
    [Key] public int Id { get; set; }

    public required string Name { get; set; }

    public bool IsActive { get; set; }
}