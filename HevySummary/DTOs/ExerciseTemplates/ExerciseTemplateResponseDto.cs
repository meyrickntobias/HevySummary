namespace HevySummary.DTOs;

public class ExerciseTemplateResponseDto(int page, int pageCount, List<ExerciseTemplateDto> exerciseTemplates)
{
    public int Page { get; set; } = page;
    public int PageCount { get; set; } = pageCount;
    public List<ExerciseTemplateDto> ExerciseTemplates { get; set; } = exerciseTemplates;
}