namespace NexoApi.DTOs.project;

public class ProjectResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int CreatedBy { get; set; }
}