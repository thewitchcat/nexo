namespace NexoApi.Models;

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string Role { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public List<Project> Projects { get; set; } = [];
    public List<Task> CreatedTasks { get; set; } = [];
    public List<Task> AssignedTasks { get; set; } = [];
}