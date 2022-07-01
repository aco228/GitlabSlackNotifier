using GitlabSlackNotifier.Core.Domain.Application.Commands;

namespace GitlabSlackNotifier.Core.Tests.Domain.Application.Commands;

public class ReportPullRequestCommandModelTest
{
    [Theory]
    [InlineData("13w", 13, DurationType.Weeks)]
    [InlineData("25d", 25, DurationType.Days)]
    [InlineData("150d", 150, DurationType.Days)]
    [InlineData("1620w", 1620, DurationType.Weeks)]
    public void Should_Convert_Duration_Correctly_For_Weeks(
        string duration, 
        int expectedVal, 
        DurationType expectedType)
    {
        var model = new ReportPullRequestCommandModel
        {
            Channel = "channel",
            Duration = duration
        };

        var result = model.GetDuration(out var period);
        
        Assert.True(result);
        Assert.Equal(expectedVal, period.Value);
        Assert.Equal(expectedType, period.Type);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("25")]
    [InlineData("25asdas")]
    [InlineData("adsasd")]
    public void Should_Fail_On_Wrong_Input(string duration)
    {
        var model = new ReportPullRequestCommandModel
        {
            Channel = "channel",
            Duration = duration
        };

        var result = model.GetDuration(out var period);
        
        Assert.False(result);
        Assert.Null(period);
    }

    [Theory]
    [InlineData(15, DurationType.Days, false)]
    [InlineData(2, DurationType.Weeks, false)]
    [InlineData(4, DurationType.Weeks, true)]
    public void Should_Correctly_CompareData(
        int value, DurationType type, bool expectedResult)
    {
        var currentDate = new DateTime(2022,7,1,0,0,0,0, DateTimeKind.Utc);
        var messageDate = new DateTime(2022,6,7,0,0,0,0, DateTimeKind.Utc); // 24 days

        var difference = (currentDate - messageDate).TotalDays;
        
        var period = new DurationPeriod
        {
            Type = type,
            Value = value,
        };

        var result = period.IsDateInPeriod(currentDate, messageDate);
        
        Assert.Equal(expectedResult, result);
    }
}