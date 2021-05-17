using System;
using System.IO;
using System.Text.RegularExpressions;
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
        
        Assert.Equal(expectedMessage,lastLine.Substring(lastLine.Length-expectedMessage.Length)); 
    }

    [Fact]
    public void Log_WritesMessageHeaderToLogTxt()
    {
        var expectedMessage = "test";
        var timeRegex = new Regex(@"\d{4}-\d{2}-\d{2}\s[0-2][0-9]:[0-5][0-9]:[0-5][0-9]");
        _sut.Log(expectedMessage);
        var lines = File.ReadAllLines(_outputPath);
        var lastLine = lines[lines.Length - 1];
        
        Assert.True(timeRegex.Match(lastLine).Success);
    }
    [Fact]
    public void Log_CreatesFileIfDoesNotExist()
    {
        File.Delete(_outputPath);
        var expectedMessage = "test";
        _sut.Log(expectedMessage);

        Assert.True(File.Exists(_outputPath));
    }

}