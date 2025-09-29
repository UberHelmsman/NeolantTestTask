namespace NeolantTestTask.Models;

public class ConsoleLogger : ICustomLogger
{
    public void Log(string level, string message)
    {
        Console.WriteLine($"[{level}] [{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}");
    }
}

public class FileLogger(string fileName) : ICustomLogger
{
    public string FileName { get; set; } = fileName;

    public void Log(string level, string message)
    {
        using (var writer = new StreamWriter(FileName, true))
        {
            writer.WriteLine($"[{level}] [{DateTime.Now:dd.MM.yyyy HH:mm:ss}] {message}\n");
        }
    }
}