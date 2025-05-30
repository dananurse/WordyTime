using WordyTime;
using Xunit.Abstractions;
using NSubstitute;

namespace WordyTests;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Test1()
    {
        var provider = Substitute.For<IDateTimeProvider>();
        var now = new DateTime(2022, 1, 1, 12, 0, 0);
        var utcnow = new DateTime(2022, 1, 1, 11, 0, 0);
        provider.Now.Returns(now);
        provider.UtcNow.Returns(utcnow);

        var testdate = new DateTime(2022, 1, 1, 7, 45, 0);
        //var testdate = utcnow.AddMinutes(-165);
        
        var formatter = new ConversationalDateTimeFormatter(provider);

        for (int i = 0; i < 10; i++)
        {
            var result = formatter.Format(testdate);
        
            _output.WriteLine(result);
        }
    }
    
    [Fact]
    public void Test2()
    {
        var provider = Substitute.For<IDateTimeProvider>();
        var now = new DateTime(2022, 1, 1, 12, 0, 0);
        var utcnow = new DateTime(2022, 1, 1, 11, 0, 0);
        provider.Now.Returns(now);
        provider.UtcNow.Returns(utcnow);
        
        var formatter = new ConversationalDateTimeFormatter(provider);

        for (int i = 0; i < 1440; i+=15)
        {
            var testdate = new DateTime(2022, 1, 1, 0, 0, 0);
            var result = formatter.Format(testdate.AddMinutes(i));
        
            //_output.WriteLine(testdate.ToShortTimeString());
            _output.WriteLine(result);
        }
    }
    
    [Fact]
    public void Test3()
    {
        var provider = Substitute.For<IDateTimeProvider>();
        var now = new DateTime(2022, 1, 23, 12, 0, 0);
        var utcnow = new DateTime(2022, 1, 23, 11, 0, 0);
        provider.Now.Returns(now);
        provider.UtcNow.Returns(utcnow);

        var testdate = new DateTime(2022, 1, 1, 7, 45, 0);
        //var testdate = utcnow.AddMinutes(-165);
        
        var formatter = new ConversationalDateTimeFormatter(provider);

        for (int i = 0; i < 10; i++)
        {
            var result = formatter.Format(testdate);
        
            _output.WriteLine(result);
        }
    }
    
    [Fact]
    public void Test4()
    {
        var provider = Substitute.For<IDateTimeProvider>();
        var now = new DateTime(2022, 1, 10, 12, 0, 0);
        var utcnow = new DateTime(2022, 1, 10, 11, 0, 0);
        provider.Now.Returns(now);
        provider.UtcNow.Returns(utcnow);
        
        var formatter = new ConversationalDateTimeFormatter(provider);

        for (int i = 0; i < 60; i+=1)
        {
            var testdate = new DateTime(2022, 1, 9, 0, 0, 0);
            var result = formatter.Format(testdate.AddHours(i));
        
            //_output.WriteLine(testdate.AddHours(i).ToString());
            _output.WriteLine(result);
        }
    }
}