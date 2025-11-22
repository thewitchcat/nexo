using Microsoft.EntityFrameworkCore;

using NexoApi.Models;

namespace NexoApi.Data;

public class NexoDb : DbContext
{
    public NexoDb(DbContextOptions<NexoDb> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Models.Task> Tasks => Set<Models.Task>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().ToTable("users");
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.Name).HasColumnName("name");
            e.Property(u => u.Email).HasColumnName("email");
            e.Property(u => u.Password).HasColumnName("password");
            e.Property(u => u.Role).HasColumnName("role");
        });

        modelBuilder.Entity<RefreshToken>().ToTable("refresh_tokens");
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.Property(rt => rt.Id).HasColumnName("id");
            e.Property(rt => rt.Token).HasColumnName("token");
            e.Property(rt => rt.Expires).HasColumnName("expires");
            e.Property(rt => rt.Created).HasColumnName("created");
            e.Property(rt => rt.Revoked).HasColumnName("revoked");
            e.Property(rt => rt.UserId).HasColumnName("user_id");
        });

        modelBuilder.Entity<Project>().ToTable("projects");
        modelBuilder.Entity<Project>(e =>
        {
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.Name).HasColumnName("name");
            e.Property(p => p.Description).HasColumnName("description");
            e.Property(p => p.CreatedBy).HasColumnName("created_by");
        });

        modelBuilder.Entity<Project>()
          .HasOne(p => p.User)
          .WithMany(u => u.Projects)
          .HasForeignKey(p => p.CreatedBy)
          .IsRequired();

        modelBuilder.Entity<Models.Task>().ToTable("tasks");
        modelBuilder.Entity<Models.Task>(e =>
        {
            e.Property(t => t.Id).HasColumnName("id");
            e.Property(t => t.Title).HasColumnName("title");
            e.Property(t => t.Description).HasColumnName("description");
            e.Property(t => t.Type).HasColumnName("type");
            e.Property(t => t.Status).HasColumnName("status");
            e.Property(t => t.Priority).HasColumnName("priority");
            e.Property(t => t.DueDate).HasColumnName("due_date");
            e.Property(t => t.ProjectId).HasColumnName("project_id");
            e.Property(t => t.CreatedBy).HasColumnName("created_by");
            e.Property(t => t.AssignedTo).HasColumnName("assigned_to");

            modelBuilder.Entity<Models.Task>()
          .HasOne(t => t.CreatedByUser)
          .WithMany(u => u.CreatedTasks)
          .HasForeignKey(t => t.CreatedBy)
          .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.Task>()
          .HasOne(t => t.AssignedToUser)
          .WithMany(u => u.AssignedTasks)
          .HasForeignKey(t => t.AssignedTo)
          .OnDelete(DeleteBehavior.Restrict);
        });
    }
}