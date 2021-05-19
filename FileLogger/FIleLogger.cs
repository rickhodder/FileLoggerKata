using System;
using System.IO;
using System.Reflection;

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
        if (currentTime.IsWeekend() 
            && File.Exists(GetPath("weekend.txt")))
        {
            var weekend = "weekend.txt";
            var created = File.GetCreationTime(GetPath(weekend));
            var lastUpdated = File.GetLastWriteTime(weekend);
            var fileDate = lastUpdated > created ? lastUpdated: created;

            if (fileDate.DayOfWeek == DayOfWeek.Sunday)
                fileDate=fileDate.AddDays(-1);
            var w = GetPath("weekend.txt");
            var x = GetPath($"weekend-{fileDate:yyyyMMdd}.txt");
            if (File.Exists(x))
            {
                
            }
            //rename to saturday of that weekend
            File.Move(GetPath("weekend.txt"),GetPath($"weekend-{fileDate:yyyyMMdd}.txt"),true);
        }

        if(currentTime.IsWeekend())
        {
            return GetPath("weekend.txt");
        }

        return GetPath( $"log{currentTime:yyyyMMdd}.txt");
    }

    private string GetPath(string fileName)
    {
        return Path.Combine(_path, fileName);
    }
}



public interface ISystemFunctions
{
    DateTime GetCurrentDateTime();
    
}

public static class Extensions
{
    public static bool IsWeekend(this DateTime date)
    {
        return date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;
    }
}