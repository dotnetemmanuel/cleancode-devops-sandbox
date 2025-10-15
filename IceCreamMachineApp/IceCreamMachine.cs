namespace IceCreamMachineApp;

public class IceCreamMachine
{
    private readonly Dictionary<string, int> scoopTable = new()
    {
        { "S", 1 },
        { "M", 3 },
        { "L", 4 },
        { "XL", 5 },
    };

    public int GetScoops(string size)
    {
        return scoopTable.TryGetValue(size, out var scoops) ? scoops : 0;
    }
}