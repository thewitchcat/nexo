namespace NexoApi.DTOs.auth;

public class LoginResponseDto
{
    public required string Email { get; set; }
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}