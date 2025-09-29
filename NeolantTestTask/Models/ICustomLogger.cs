namespace NeolantTestTask.Models;

public interface ICustomLogger
{
    public void Log(string level, string message);
}