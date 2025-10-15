using System.Globalization;

namespace TestabilityDemo.Core;

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}

public class Database : IDatabase
{
    public float GetItemPrice(string item) => new Random().Next(1, 10);
}

public class ConsolePrinter : IPrinter
{
    public void Print(string text) => Console.WriteLine(text);
}