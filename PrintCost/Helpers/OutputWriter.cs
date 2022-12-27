using System;

namespace PrintCost.Helpers
{
  public interface IOutputWriter
  {
    public void ConsoleWriteLine(string message);
  }

  public class OutputWriter : IOutputWriter
  {
    public void ConsoleWriteLine(string message)
    {
      Console.WriteLine(message);
    }
  }
}