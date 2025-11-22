namespace NexoApi.Models;

public class Task
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
    public required string Priority { get; set; }
    public DateOnly DueDate { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; }

    public int CreatedBy { get; set; }
    public User CreatedByUser { get; set; }

    public int? AssignedTo { get; set; }
    public User AssignedToUser { get; set; }
}