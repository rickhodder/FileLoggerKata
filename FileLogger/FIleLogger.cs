using System;
using System.IO;

public class FileLogger
{
    private readonly string _path;

    public FileLogger(string path)
    {
        _path = path;
    }

    public void Log(string message)
    {
        var fileName = $"{DateTime.Now:yyyyMMdd}.txt";
        File.AppendAllLines(Path.Combine(_path,fileName), new string[] {$"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}"});
    }
}

