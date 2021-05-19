using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Xunit;

public class FileLoggerTests
{
    private FileLogger _sut { get; set; }
    private readonly string _outputPath = Environment.CurrentDirectory;
    private readonly FakeSystemFunctions _systemFunctions;


    public FileLoggerTests()
    {
        _systemFunctions = new FakeSystemFunctions { CurrentDateTime = GetAWeekday() };
        _sut = new FileLogger(_outputPath, _systemFunctions);
        CleanUpLogs();
    }

    private void CleanUpLogs()
    {
        CleanUpFiles("log*.txt");
        CleanUpFiles("weekend*.txt");
    }

    private void CleanUpFiles(string searchPattern)
    {
        foreach (var file in Directory.GetFiles(_outputPath, searchPattern))
        {
            File.Delete(file);
        }
    }

    [Theory]
    [InlineData("2021/05/15", "2021/05/22")] // saturday one week apart
    [InlineData("2021/05/15", "2021/05/29")] // saturday two weeks apart

    public void Log_TwoSeparateWeekends_CopiesAndCreatesNewWeekend(string previousWeekendDay, string testWeekendDay)
    {
        var previousWeekend = DateTime.Parse(previousWeekendDay);
        var nextWeekend = DateTime.Parse(testWeekendDay);

        _systemFunctions.CurrentDateTime = previousWeekend;

        var weekendFile = GetPath("weekend.txt");

        // if i dont put this in using, it locks
        using (var _ = File.Create(weekendFile))
        {
        }

        File.SetCreationTime(weekendFile, previousWeekend);
        File.SetLastWriteTime(weekendFile, previousWeekend);

        var expectedMessage = "test";
        _systemFunctions.CurrentDateTime = nextWeekend;

        _sut.Log(expectedMessage);

        var expectedFile = GetPath($"weekend-{previousWeekend:yyyyMMdd}.txt");
        Assert.True(File.Exists(weekendFile), $"Expected file {weekendFile} didnt exist");
        Assert.True(File.Exists(expectedFile), $"Expected file {expectedFile} didnt exist");
    }

    [Fact]
    public void Log_VerifyLoggingOnDifferentDaysCreatesDifferentFiles()
    {
        var expectedFilePath = GetExpectedFilePath();

        var expectedMessage = "test";
        _sut.Log(expectedMessage);

        _systemFunctions.CurrentDateTime = _systemFunctions.CurrentDateTime.AddDays(1);

        var expectedFilePath2 = GetExpectedFilePath();

        _sut.Log(expectedMessage);

        Assert.True(File.Exists(expectedFilePath2), $"Expected file {expectedFilePath2} didnt exist");
    }

    [Fact]
    public void Log_CreatesLogFileBasedOnDateWeekendSaturday()
    {
        _systemFunctions.CurrentDateTime = GetASaturday();

        var expectedFilePath = GetExpectedFilePath();

        var expectedMessage = "test";
        _sut.Log(expectedMessage);

        Assert.True(expectedFilePath.EndsWith("weekend.txt", StringComparison.CurrentCultureIgnoreCase));
        Assert.True(File.Exists(expectedFilePath), $"Expected file {expectedFilePath} didnt exist");
    }

    [Fact]
    public void Log_CreatesLogFileBasedOnDateWeekendSunday()
    {
        _systemFunctions.CurrentDateTime = GetASunday();

        var expectedFilePath = GetExpectedFilePath();

        var expectedMessage = "test";
        _sut.Log(expectedMessage);

        Assert.True(expectedFilePath.EndsWith("weekend.txt", StringComparison.CurrentCultureIgnoreCase));
        Assert.True(File.Exists(expectedFilePath), $"Expected file {expectedFilePath} didnt exist");
    }

    [Fact]
    public void Log_CreatesLogFileBasedOnDateWeekday()
    {
        var expectedFilePath = GetExpectedFilePath();

        var expectedMessage = "test";
        _sut.Log(expectedMessage);

        Assert.True(File.Exists(expectedFilePath), $"Expected file {expectedFilePath} didnt exist");
    }

    [Fact]
    public void Log_WritesMessageToLogTxt()
    {
        var expectedMessage = "test";
        _sut.Log(expectedMessage);
        var lines = File.ReadAllLines(GetExpectedFilePath());
        var lastLine = lines[lines.Length - 1];

        Assert.Equal(expectedMessage, lastLine.Substring(lastLine.Length - expectedMessage.Length));
    }

    [Fact]
    public void Log_WritesMessageHeaderToLogTxt()
    {
        var expectedMessage = "test";
        var timeRegex = new Regex(@"\d{4}-\d{2}-\d{2}\s[0-2][0-9]:[0-5][0-9]:[0-5][0-9]");
        _sut.Log(expectedMessage);
        var lines = File.ReadAllLines(GetExpectedFilePath());
        var lastLine = lines[lines.Length - 1];

        Assert.True(timeRegex.Match(lastLine).Success);
    }
    [Fact]
    public void Log_CreatesFileIfDoesNotExist()
    {
        GetAWeekday();

        var expectedFilePath = GetExpectedFilePath();

        var expectedMessage = "test";
        _sut.Log(expectedMessage);

        Assert.True(File.Exists(expectedFilePath), $"Expected file {expectedFilePath} didnt exist");
    }

    private DateTime GetAWeekday()
    {
        return new DateTime(2021, 5, 17); // weekday
    }

    private DateTime GetASaturday()
    {
        return new DateTime(2021, 5, 15);
    }

    private DateTime GetASunday()
    {
        return new DateTime(2021, 5, 16); // weekday
    }

    private string GetExpectedFilePath()
    {
        var currentTime = _systemFunctions.GetCurrentDateTime();
        if (currentTime.IsWeekend())
        {
            if (File.Exists(GetPath("weekend.txt")))
            {
                var weekend = "weekend.txt";
                var created = File.GetCreationTime(GetPath(weekend));
                var lastUpdated = File.GetLastWriteTime(weekend);
                var fileDate = lastUpdated > created ? lastUpdated : created;

                if (fileDate.DayOfWeek == DayOfWeek.Sunday)
                    fileDate = fileDate.AddDays(-1);

                //rename to saturday of that weekend
                return GetPath($"weekend-{fileDate:yyyyMMdd}.txt");
            }

            return GetPath("weekend.txt");
        }

        return GetPath($"log{_systemFunctions.GetCurrentDateTime():yyyyMMdd}.txt");
    }

    private string GetPath(string fileName)
    {
        return Path.Combine(_outputPath, fileName);
    }

}

public class FakeSystemFunctions : ISystemFunctions
{
    public DateTime CurrentDateTime { get; set; } = DateTime.Now;

    public DateTime GetCurrentDateTime()
    {
        return CurrentDateTime;
    }
}