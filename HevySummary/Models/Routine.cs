using HevySummary.DTOs;

namespace HevySummary.Models;

public class Routine(
    string title, 
    string? notes, 
    List<Exercise> exercises, 
    DateTime createdAt,
    DateTime updatedAt)
{
    public string Title { get; set; } = title;
    public string? Notes { get; set; } = notes;
    public List<Exercise> Exercises { get; set; } = exercises;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime UpdatedAt { get; set; } = updatedAt;
}