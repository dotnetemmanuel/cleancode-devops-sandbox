namespace TestabilityDemo.Core;

public interface IClock
{
    DateTime Now { get; }
}

public interface IDatabase
{
    float GetItemPrice(string item);
}

public interface IPrinter
{
    void Print(string text);
}