namespace NexoApi.DTOs.user;

public class UserUpdateRequestDto
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }
}