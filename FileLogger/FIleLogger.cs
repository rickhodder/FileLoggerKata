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
        File.AppendAllLines(_path,new string[] { $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}"});
    }
}