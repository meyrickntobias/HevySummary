namespace HevySummary.DTOs;

public class ExerciseDto
{
    public required string Title { get; set; }
    public required string ExerciseTemplateId { get; set; }
    public List<ExerciseSetDto> Sets { get; set; } = [];
}

public class ExerciseDtoEqualityComparer : IEqualityComparer<ExerciseDto>
{
    public bool Equals(ExerciseDto? x, ExerciseDto? y)
    {
        return x?.ExerciseTemplateId == y?.ExerciseTemplateId;
    }

    public int GetHashCode(ExerciseDto obj)
    {
        return HashCode.Combine(obj.ExerciseTemplateId);
    }
}