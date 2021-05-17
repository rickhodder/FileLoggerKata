using System;
using System.IO;

public class FileLogger
{
    private readonly ISystemFunctions _systemFunctions;
    private readonly string _path;

    public FileLogger(string path,ISystemFunctions systemFunctions)
    {
        _path = path;
        _systemFunctions = systemFunctions;
    }

    public void Log(string message)
    {
        var currentTime = _systemFunctions.GetCurrentDateTime();
        var fileName = GetLogFileName(currentTime);
        File.AppendAllLines(fileName, new[] {$"{currentTime:yyyy-MM-dd HH:mm:ss} {message}"});
    }

    private string GetLogFileName(DateTime currentTime)
    {
        if(currentTime.DayOfWeek == DayOfWeek.Saturday || currentTime.DayOfWeek == DayOfWeek.Sunday)
        {
            return Path.Combine(_path, "weekend.txt");
        }

        return Path.Combine(_path, $"{currentTime:yyyyMMdd}.txt");
    }
}

public interface ISystemFunctions
{
    DateTime GetCurrentDateTime();
    
}