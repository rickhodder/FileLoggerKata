using System.IO;
using Xunit;

public class FileLoggerTests
{
    private FileLogger _sut { get; set; }
    private string _outputPath = "log.txt";
    public FileLoggerTests()
    {
        _sut = new FileLogger(_outputPath);
    }

    [Fact]
    public void Log_WritesMessageToLogTxt()
    {
        var expectedMessage = "test";
        _sut.Log(expectedMessage);
        var lines = File.ReadAllLines(_outputPath);
        var lastLine = lines[lines.Length-1];
        
        Assert.Equal("test",lastLine.Substring(lastLine.Length-4)); 
    }
}