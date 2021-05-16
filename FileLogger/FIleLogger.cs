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
        File.AppendAllLines(_path,new string[] {message});
    }
}