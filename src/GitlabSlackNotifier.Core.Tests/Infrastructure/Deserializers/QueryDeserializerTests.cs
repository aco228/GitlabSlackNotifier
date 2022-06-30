using GitlabSlackNotifier.Core.Infrastructures.Deserializers;
using Newtonsoft.Json;

namespace GitlabSlackNotifier.Core.Tests.Infrastructure.Deserializers;

public class QueryDeserializerTests
{
    [Fact]
    public void Should_Fail_When_Input_Is_Empty()
    {
        var deserializer = GetDeserializer();
        var result = deserializer.TryDeserialize<DummyQueryDeserializerModel>("", out var response);
        Assert.False(result);
    }

    [Fact]
    public void Should_Fail_When_Query_Is_Not_Correct()
    {
        var deserializer = GetDeserializer();
        var result = deserializer.TryDeserialize<DummyQueryDeserializerModel>("aco&pero=nije", out var response);
        Assert.False(result);
    }

    [Fact]
    public void Should_Convert_Query()
    {
        var deserializer = GetDeserializer();
        var query = "val1=https://hooks.slack.com/&val2=25&val3=false";
        
        var result = deserializer.TryDeserialize<DummyQueryDeserializerModel>(query, out var response);
        Assert.True(result);
        Assert.NotNull(response);
        
        Assert.Equal("https://hooks.slack.com/", response.Value1);
        Assert.Equal(25, response.Value2);
        Assert.False(response.Value3);
    }


    private QueryDeserializer GetDeserializer()
    {
        return new QueryDeserializer();
    }
}

public class DummyQueryDeserializerModel
{
    [JsonProperty("val1")]
    public string Value1 { get; set; }
    
    [JsonProperty("val2")]
    public int Value2 { get; set; }
    
    [JsonProperty("val3")]
    public bool Value3 { get; set; }
}