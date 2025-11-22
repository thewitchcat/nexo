using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using NexoApi.Data;
using NexoApi.DTOs.task;

namespace NexoApi.Controllers;

public static class TaskController
{
    public static void RegisterTaskController(this WebApplication app)
    {
        var route = app.MapGroup("/tasks");

        route.MapPost("/", CreateTaskAsync)
          .WithSummary("Create task")
          .WithDescription("Create a new task")
          .WithTags("Tasks")
          .RequireAuthorization();

        route.MapGet("/", GetTasksAsync)
          .WithSummary("Get tasks")
          .WithDescription("Get list of tasks")
          .WithTags("Tasks")
          .RequireAuthorization();

        route.MapGet("/{id}", GetTaskByIdAsync)
          .WithSummary("Get task by ID")
          .WithDescription("Get a specific task by their ID")
          .WithTags("Tasks")
          .RequireAuthorization();

        route.MapPut("/{id}", UpdateTaskByIdAsync)
          .WithSummary("Update task by ID")
          .WithDescription("Update task's data based on their ID")
          .WithTags("Tasks")
          .RequireAuthorization();

        route.MapDelete("/{id}", DeleteTaskByIdAsync)
          .WithSummary("Delete task")
          .WithDescription("Delete a task based on their ID")
          .WithTags("Tasks")
          .RequireAuthorization();

        // I'm too lazy to implement this, I'll do it later 😂
        // PATCH "/tasks/:taskId"
        // PATCH "/tasks/:taskId/assign"
        // PATCH "/tasks/:taskId/status"
        // PATCH "/tasks/:taskId/priority"

        // GET "/tasks?projectId=1"
        // GET "/tasks?assignedTo=6"
        // GET "/tasks?status=open"
        // GET "/tasks?type=bug"
        // GET "/tasks?priority=high"


        static async Task<Created<TaskResponseDto>> CreateTaskAsync(NexoDb db, TaskRequestDto payload)
        {
            var task = new Models.Task
            {
                Title = payload.Title,
                Description = payload.Description,
                Type = payload.Type,
                Status = payload.Status,
                Priority = payload.Priority,
                DueDate = payload.DueDate,
                ProjectId = payload.ProjectId,
                CreatedBy = payload.CreatedBy,
                AssignedTo = payload.AssignedTo
            };

            db.Tasks.Add(task);
            await db.SaveChangesAsync();

            var res = new TaskResponseDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Type = task.Type,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                ProjectId = task.ProjectId,
                CreatedBy = task.CreatedBy,
                AssignedTo = task.AssignedTo
            };

            return TypedResults.Created($"/tasks/{res.Id}", res);
        }

        static async Task<Ok<TaskResponseDto[]>> GetTasksAsync(NexoDb db)
        {
            var tasks = await db.Tasks.ToListAsync();
            var res = tasks.Select(t => new TaskResponseDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Type = t.Type,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                ProjectId = t.ProjectId,
                CreatedBy = t.CreatedBy,
                AssignedTo = t.AssignedTo
            }).ToArray();

            return TypedResults.Ok(res);
        }

        static async Task<Results<Ok<TaskResponseDto>, NotFound>> GetTaskByIdAsync(NexoDb db, int id)
        {
            if (await db.Tasks.FindAsync(id) is Models.Task task)
            {
                var res = new TaskResponseDto
                {
                    Id = task.Id,
                    Title = task.Title,
                    Description = task.Description,
                    Type = task.Type,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    ProjectId = task.ProjectId,
                    CreatedBy = task.CreatedBy,
                    AssignedTo = task.AssignedTo
                };

                return TypedResults.Ok(res);
            }

            return TypedResults.NotFound();
        }

        static async Task<Results<NoContent, NotFound>> UpdateTaskByIdAsync(NexoDb db, int id, TaskRequestDto payload)
        {
            if (await db.Tasks.FindAsync(id) is Models.Task task)
            {
                task.Title = payload.Title;
                task.Description = payload.Description;
                task.Type = payload.Type;
                task.Status = payload.Status;
                task.Priority = payload.Priority;
                task.DueDate = payload.DueDate;
                task.ProjectId = payload.ProjectId;
                task.CreatedBy = payload.CreatedBy;
                task.AssignedTo = payload.AssignedTo;

                await db.SaveChangesAsync();

                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }

        static async Task<Results<NoContent, NotFound>> DeleteTaskByIdAsync(NexoDb db, int id)
        {
            if (await db.Tasks.FindAsync(id) is Models.Task task)
            {
                db.Tasks.Remove(task);
                await db.SaveChangesAsync();
                return TypedResults.NoContent();
            }

            return TypedResults.NotFound();
        }
    }
}