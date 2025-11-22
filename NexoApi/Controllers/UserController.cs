using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using NexoApi.Data;
using NexoApi.DTOs.user;
using NexoApi.Models;

namespace NexoApi.Controllers;

public static class UserController
{
    public static void RegisterUserController(this WebApplication app)
    {
        var route = app.MapGroup("/users");

        route.MapPost("/", CreateUserAsync)
          .WithSummary("Create user")
          .WithDescription("Create a new user (only user with `pm` role can add using this endpoint)")
          .WithTags("Users")
          .RequireAuthorization();

        route.MapGet("/", GetUsersAsync)
          .WithSummary("Get users")
          .WithDescription("Get list of users")
          .WithTags("Users")
          .RequireAuthorization();

        route.MapGet("/{id}", GetUserByIdAsync)
          .WithSummary("Get user by ID")
          .WithDescription("Get a specific user by their ID")
          .WithTags("Users")
          .RequireAuthorization();

        route.MapPut("/{id}", UpdateUserByIdAsync)
          .WithSummary("Update user by ID")
          .WithDescription("Update user's data based on their ID")
          .WithTags("Users")
          .RequireAuthorization();

        route.MapDelete("/{id}", DeleteUserByIdAsync)
          .WithSummary("Delete user")
          .WithDescription("Delete a user based on their ID")
          .WithTags("Users")
          .RequireAuthorization();

        // I'm too lazy to implement this, I'll do it later 😂
        // GET "/users?role=employee"
        // GET "/users?role=pm"
        // PATCH "/users/{userId}/change-password

    }

    static async Task<Created<UserResponseDto>> CreateUserAsync(NexoDb db, UserRequestDto payload)
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

    static async Task<Ok<UserResponseDto[]>> GetUsersAsync(NexoDb db)
    {
        var users = await db.Users.ToListAsync();
        var res = users.Select(u => new UserResponseDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role
        }).ToArray();

        return TypedResults.Ok(res);
    }

    static async Task<Results<Ok<UserResponseDto>, NotFound>> GetUserByIdAsync(NexoDb db, int id)
    {
        if (await db.Users.FindAsync(id) is User user)
        {
            var res = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };

            return TypedResults.Ok(res);
        }

        return TypedResults.NotFound();
    }

    static async Task<Results<NoContent, NotFound>> UpdateUserByIdAsync(NexoDb db, int id, UserUpdateRequestDto payload)
    {
        if (await db.Users.FindAsync(id) is User user)
        {
            user.Name = payload.Name;
            user.Email = payload.Email;
            user.Role = payload.Role;

            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }

    static async Task<Results<NoContent, NotFound>> DeleteUserByIdAsync(NexoDb db, int id)
    {
        if (await db.Users.FindAsync(id) is User user)
        {
            db.Users.Remove(user);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound();
    }
}