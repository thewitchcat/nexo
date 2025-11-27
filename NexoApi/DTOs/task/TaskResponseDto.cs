namespace NexoApi.DTOs.task;

public class TaskResponseDto
{
    public required int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Type { get; set; }
    public required string Status { get; set; }
    public required string Priority { get; set; }
    public DateOnly DueDate { get; set; }
    public required string ProjectId { get; set; }
    public required string CreatedBy { get; set; }
    public string? AssignedTo { get; set; }
}