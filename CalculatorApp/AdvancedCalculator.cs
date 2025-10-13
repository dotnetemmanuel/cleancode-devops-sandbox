namespace CalculatorApp;

public class AdvancedCalculator
{
    public string Owner { get; set; }
    public string Model { get; set; }
    public bool IsScientific { get; set; }

    public int Add(int a, int b) => a + b;
}