namespace HevyDashboard.UnitTest;

public class Tests
{

    [Test]
    public void Test1()
    {
        var dates = new DateHelper().GetDateRanges(4);
        Console.WriteLine(dates);
    }
}