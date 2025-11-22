using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using NexoApi.Data;
using NexoApi.DTOs.auth;
using NexoApi.DTOs.user;
using NexoApi.Models;
using NexoApi.Services.interfaces;

namespace NexoApi.Controllers;

public static class AuthController
{
    public static void RegisterAuthController(this WebApplication app)
    {
        var route = app.MapGroup("/auth");

        route.MapPost("/register", Register)
          .WithSummary("Register")
          .WithDescription("Register a new account")
          .WithTags("Auth");

        route.MapPost("/login", Login)
          .WithSummary("Login")
          .WithDescription("Login to the app")
          .WithTags("Auth");

        route.MapPost("/logout", Logout)
          .WithSummary("Logout")
          .WithDescription("Logout from the app")
          .WithTags("Auth")
          .RequireAuthorization();

        route.MapPost("/refresh-token", RefreshAccessToken)
          .WithSummary("Refresh token")
          .WithDescription("Ask for refreshing a JWT Token")
          .WithTags("Auth");

        route.MapGet("/me", GetCurrentLoggedInUser)
          .WithSummary("Me")
          .WithDescription("Get current information about logged-in user")
          .WithTags("Auth")
          .RequireAuthorization();

        static async Task<Created<UserResponseDto>> Register(NexoDb db, UserRequestDto payload)
        {
            var user = new User
            {
                Name = payload.Name,
                Email = payload.Email,
                Password = payload.Password,
                Role = payload.Role
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            var res = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return TypedResults.Created($"/users/{res.Id}", res);
        }

        static async Task<Results<Ok<LoginResponseDto>, UnauthorizedHttpResult>> Login(NexoDb db, LoginRequestDto payload, IJwtService jwtService)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);
            if (user == null || user.Password != payload.Password) return TypedResults.Unauthorized();

            var accessToken = jwtService.GenerateAccessToken(user);
            var refreshToken = jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                User = user
            };

            user.RefreshTokens.Add(refreshTokenEntity);
            await db.SaveChangesAsync();

            var res = new LoginResponseDto
            {
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            return TypedResults.Ok(res);
        }

        static async Task<Results<Ok<LoginResponseDto>, UnauthorizedHttpResult>> RefreshAccessToken(NexoDb db, RefreshTokenRequestDto payload, IJwtService jwtService)
        {
            var user =
              await db.Users.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == payload.RefreshToken));

            if (user == null) return TypedResults.Unauthorized();

            var existingToken = user.RefreshTokens.First(rt => rt.Token == payload.RefreshToken);
            if (!existingToken.IsActive) return TypedResults.Unauthorized();

            existingToken.Revoked = DateTime.UtcNow;

            var newRefreshToken = jwtService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = newRefreshToken,
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                User = user
            };

            user.RefreshTokens.Add(refreshTokenEntity);

            var accessToken = jwtService.GenerateAccessToken(user);
            await db.SaveChangesAsync();

            var res = new LoginResponseDto
            {
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = newRefreshToken
            };

            return TypedResults.Ok(res);
        }

        static async Task<Results<Ok<LogoutResponseDto>, UnauthorizedHttpResult>> Logout(NexoDb db, HttpContext http)
        {
            var userIdClaim = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) return TypedResults.Unauthorized();
            if (!int.TryParse(userIdClaim, out var userId)) return TypedResults.Unauthorized();

            var tokens = db.RefreshTokens.Where(rt => rt.UserId == userId);

            db.RefreshTokens.RemoveRange(tokens);
            await db.SaveChangesAsync();

            var res = new LogoutResponseDto
            {
                Message = "Logged out successfully"
            };

            return TypedResults.Ok(res);
        }

        static async Task<Results<Ok<UserResponseDto>, UnauthorizedHttpResult>> GetCurrentLoggedInUser(NexoDb db, HttpContext http)
        {
            var userId = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return TypedResults.Unauthorized();
            }

            var user = await db.Users.FindAsync(int.Parse(userId));
            var res = new UserResponseDto
            {
                Id = user!.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return TypedResults.Ok(res);
        }
    }
}