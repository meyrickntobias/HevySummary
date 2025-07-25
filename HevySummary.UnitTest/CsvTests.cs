using HevySummary.Services;

namespace HevySummary.UnitTest;

public class CsvTests
{
    [Fact]
    public void TestCsvFile()
    {
        var csvService = new CsvService();
        var csvLines= File.ReadLines(@"C:\dev\personal_dev\HevySummary\HevySummary\HevySummary.UnitTest\test-workouts.csv");
        
        csvService.ImportHevyCsv(csvLines.ToArray());
    }
}