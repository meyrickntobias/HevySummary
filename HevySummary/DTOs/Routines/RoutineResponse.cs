using HevySummary.DTOs.Routines;

namespace HevySummary.DTOs;

public class RoutineResponse(int page, int pageCount, List<RoutineDto> routines)
{
    public int Page { get; set; } = page;
    public int PageCount { get; set; } = pageCount;
    public List<RoutineDto> Routines { get; set; } = routines;
}