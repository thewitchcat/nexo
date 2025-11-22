using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using NexoApi.Data;
using NexoApi.DTOs.project;
using NexoApi.Models;

namespace NexoApi.Controllers;

public static class ProjectController
{
    public static void RegisterProjectController(this WebApplication app)
    {
        var route = app.MapGroup("/projects");

        route.MapPost("/", CreateProjectAsync)
          .WithSummary("Create project")
          .WithDescription("Create a new project")
          .WithTags("Projects")
          .RequireAuthorization();

        route.MapGet("/", GetProjectsAsync)
          .WithSummary("Get projects")
          .WithDescription("Get list of projects")
          .WithTags("Projects")
          .RequireAuthorization();

        route.MapGet("/{id}", GetProjectByIdAsync)
          .WithSummary("Get project by ID")
          .WithDescription("Get a specific project by their ID")
          .WithTags("Projects")
          .RequireAuthorization();

        route.MapPut("/{id}", UpdateProjectByIdAsync)
          .WithSummary("Update project by ID")
          .WithDescription("Update project's data based on their ID")
          .WithTags("Projects")
          .RequireAuthorization();

        route.MapDelete("/{id}", DeleteProjectByIdAsync)
          .WithSummary("Delete project")
          .WithDescription("Delete a project based on their ID")
          .WithTags("Projects")
          .RequireAuthorization();

        // I'm too lazy to implement this, I'll do it later 😂
        // GET "/projects/:projectId/tasks"

        static async Task<Created<ProjectResponseDto>> CreateProjectAsync(NexoDb db, ProjectRequestDto payload)
        {
            var project = new Project
            {
                Name = payload.Name,
                Description = payload.Description,
                CreatedBy = payload.CreatedBy,
            };

            db.Projects.Add(project);
            await db.SaveChangesAsync();

            var res = new ProjectResponseDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedBy = project.CreatedBy
            };

            return TypedResults.Created($"/projects/{res.Id}", res);
        }

        static async Task<Ok<ProjectResponseDto[]>> GetProjectsAsync(NexoDb db)
        {
            var projects = await db.Projects.ToListAsync();
            var res = projects.Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedBy = p.CreatedBy
            }).ToArray();

            return TypedResults.Ok(res);
        }

        static async Task<Results<Ok<ProjectResponseDto>, NotFound>> GetProjectByIdAsync(NexoDb db, int id)
        {
            if (await db.Projects.FindAsync(id) is Project project)
            {
                var res = new ProjectResponseDto
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedBy = project.CreatedBy
                };

                return TypedResults.Ok(res);
            }

            return TypedResults.NotFound();
        }

        static async Task<Results<NoContent, NotFound>> UpdateProjectByIdAsync(NexoDb db, int id, ProjectRequestDto payload)
        {
            if (await db.Projects.FindAsync(id) is Project project)
            {
                project.Name = payload.Name;
                project.Description = payload.Description;
                project.CreatedBy = payload.CreatedBy;

                await db.SaveChangesAsync();

                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }

        static async Task<Results<NoContent, NotFound>> DeleteProjectByIdAsync(NexoDb db, int id)
        {
            if (await db.Projects.FindAsync(id) is Project project)
            {
                db.Projects.Remove(project);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }
    }
}