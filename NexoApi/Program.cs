using System.Text;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using NexoApi.Controllers;
using NexoApi.Data;
using NexoApi.Models;
using NexoApi.Services;
using NexoApi.Services.interfaces;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowNuxt",
    builder => builder
      .WithOrigins("http://localhost:3000")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials());
});

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new();
        document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.Models.OpenApiSecurityScheme>();
        document.Components.SecuritySchemes["BearerAuth"] = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Name = "Authorization",
            Description = "JWT Bearer Authenticaton",
        };

        document.SecurityRequirements.Add(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
      {
      {
        document.Components.SecuritySchemes["BearerAuth"],
        new List<string>()
      }
      });

        return System.Threading.Tasks.Task.CompletedTask;
    });
});

builder.Services.AddDbContext<NexoDb>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DatabaseUrl")));

builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
      };
  });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<NexoDb>();
  bool hasData = db.Users.Any() || db.Projects.Any() || db.Tasks.Any();

  if (!hasData)
  {
    var users = new List<User>
    {
      new() { Name = "Alice Johnson", Email = "alice.johnson@example.com", Password = "alice123", Role = "pm" },
      new() { Name = "Brian Carter", Email = "brian.carter@example.com", Password = "brian123", Role = "pm" },
      new() { Name = "Chloe Ramirez", Email = "chloe.ramirez@example.com", Password = "chloe123", Role = "employee" },
      new() { Name = "David Kim", Email = "david.kim@example.com", Password = "david123", Role = "employee" },
      new() { Name = "Emma Brooks", Email = "emma.brooks@example.com", Password = "emma123", Role = "employee" },
    };
    db.Users.AddRange(users);

    var projects = new List<Project>
    {
      new() { Name = "Hapi API Migration", Description = "A backend project migrating an old Express API to Hapi with modular routing.", CreatedBy = 1 },
      new() { Name = "Frontend Dashboard", Description = "A responsive dashboard UI built using React and TailwindCSS.", CreatedBy = 1 },
      new() { Name = "User Management System", Description = "A user CRUD system with roles, permissions, and authentication support.", CreatedBy = 2 },
      new() { Name = "Project Tracker", Description = "A simple project tracking app with status updates and team assignments.", CreatedBy = 2 },
      new() { Name = "Internal Tools Suite", Description = "Collection of internal utilities including data viewer and admin panel.", CreatedBy = 2 },
    };
    db.Projects.AddRange(projects);

    var tasks = new List<NexoApi.Models.Task>
    {
      new() { Title = "Fix login bug", Description = "Users cannot log in when using special characters in their password.", Type = "bug", Status = "open", Priority = "high", DueDate = DateOnly.Parse("2025-12-01"), ProjectId = 1, CreatedBy = 2, AssignedTo = 3 },
      new() { Title = "Add search feature", Description = "Implement search functionality for projects and tasks.", Type = "task", Status = "in_progress", Priority = "medium", DueDate = DateOnly.Parse("2025-11-25"), ProjectId = 2, CreatedBy = 1, AssignedTo = 4 },
      new() { Title = "Fix CSS overlap", Description = "Header overlaps content on mobile screens.", Type = "bug", Status = "blocked", Priority = "low", DueDate = DateOnly.Parse("2025-12-05"), ProjectId = 3, CreatedBy = 1, AssignedTo = 5 },
      new() { Title = "Setup notifications", Description = "Enable email notifications for task assignments and status changes.", Type = "task", Status = "open", Priority = "high", DueDate = DateOnly.Parse("2025-11-28"), ProjectId = 4, CreatedBy = 1, AssignedTo = 5 },
      new() { Title = "Fix API timeout", Description = "Some endpoints take too long to respond under heavy load.", Type = "bug", Status = "in_progress", Priority = "critical", DueDate = DateOnly.Parse("2025-11-30"), ProjectId = 5, CreatedBy = 1, AssignedTo = 3 },
      new() { Title = "Update README", Description = "Document the new Hapi routes and project setup instructions.", Type = "task", Status = "done", Priority = "low", DueDate = DateOnly.Parse("2025-11-22"), ProjectId = 1, CreatedBy = 1, AssignedTo = 4 },
      new() { Title = "Fix broken links", Description = "Several internal links on the dashboard lead to 404 pages.", Type = "bug", Status = "open", Priority = "medium", DueDate = DateOnly.Parse("2025-12-02"), ProjectId = 2, CreatedBy = 2, AssignedTo = 5 },
      new() { Title = "Implement dark mode", Description = "Allow users to toggle dark/light mode in settings.", Type = "task", Status = "in_progress", Priority = "medium", DueDate = DateOnly.Parse("2025-12-07"), ProjectId = 3, CreatedBy = 2, AssignedTo = 3 },
      new() { Title = "Fix notification duplication", Description = "Users receive duplicate notifications for the same event.", Type = "bug", Status = "blocked", Priority = "high", DueDate = DateOnly.Parse("2025-12-03"), ProjectId = 4, CreatedBy = 2, AssignedTo = 4 },
      new() { Title = "Refactor codebase", Description = "Clean up legacy code and improve maintainability.", Type = "task", Status = "closed", Priority = "critical", DueDate = DateOnly.Parse("2025-12-10"), ProjectId = 5, CreatedBy = 2, AssignedTo = 5 },
    };
    db.Tasks.AddRange(tasks);

    db.SaveChanges();
    Console.WriteLine("Database seeded! ⚡");
  }
  else
  {
    Console.WriteLine("Database already has data. Skipping seeding.");
  }
}

app.UseCors("AllowNuxt");
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.WithTheme(ScalarTheme.Kepler));
}

app.MapGet("/", () => "Hello from Nexo 😁");

app.RegisterAuthController();
app.RegisterUserController();
app.RegisterProjectController();
app.RegisterTaskController();

app.Run();