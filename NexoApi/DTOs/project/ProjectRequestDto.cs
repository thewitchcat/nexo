namespace NexoApi.DTOs.project;

public class ProjectRequestDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int CreatedBy { get; set; }
}