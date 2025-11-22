using NexoApi.Models;

namespace NexoApi.Services.interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
}