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
        var fileName = $"{currentTime:yyyyMMdd}.txt";
        File.AppendAllLines(Path.Combine(_path,fileName), new[] {$"{currentTime:yyyy-MM-dd HH:mm:ss} {message}"});
    }
}

public interface ISystemFunctions
{
    DateTime GetCurrentDateTime();
    
}