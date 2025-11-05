namespace IceCreamMachineApp.Tests;

public class IceCreamMachineAppTests
{
    [Fact]
    public void GetScoops_ShouldReturnFive_ForXLSize()
    {
        //Arrange
        var machine = new IceCreamMachine();

        //Act
        var result = machine.GetScoops("XL");

        //Assert
        Assert.Equal(5, result);
    }
    
    [Theory]
    [InlineData("S", 1)]
    [InlineData("M", 3)]
    [InlineData("L", 4)]
    [InlineData("XL", 5)]
    [InlineData("XXL", 0)] // ogiltig storlek
    public void GetScoops_ShouldReturnCorrectValue(string size, int expected)
    {
        var machine = new IceCreamMachine();
        var result = machine.GetScoops(size);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetPrice_ShouldReturn50_ForXL()
    {
        var machine = new IceCreamMachine();
        var price = machine.GetPrice("XL");
        
        Assert.Equal(50, price);
    }

    [Theory]
    [InlineData("S", 10)]
    [InlineData("M", 30)]
    [InlineData("L", 40)]
    [InlineData("XL", 50)]
    [InlineData("XXL", 0)] //ogiltig storlek
    public void GetPrice_ShouldReturnCorrectPrice(string size, int expected)
    {
        var machine = new IceCreamMachine();
        var price = machine.GetPrice(size);
        
        Assert.Equal(expected, price);
    }
    
    [Fact]
    public void IsValidSize_ShouldReturnTrue_ForM()
    {
        var machine = new IceCreamMachine();
        var result = machine.IsValidSize("M");

        Assert.True(result);
    }
    
    [Theory]
    [InlineData("S", true)]
    [InlineData("M", true)]
    [InlineData("L", true)]
    [InlineData("XL", true)]
    [InlineData("XXL", false)]
    [InlineData("", false)]
    public void IsValidSize_ShouldReturnExpectedResult(string size, bool expected)
    {
        var machine = new IceCreamMachine();
        var result = machine.IsValidSize(size);

        Assert.Equal(expected, result);
    }
} 












/* Steps
 * 1. identify problems
 *      a. All code in main
 *      b. Database dependency
 *      c. DateTime.now
 *      d. Console.WriteLine
 *
 * 2. Extract code to PrintReceipt
 *
 * 3. Solve Database
 *      a. Adding interface
 *      b. Adding mock of interface
 *
 * 4. Solve DateTime.Now
 *      a. Add wrapper class and Interface
 *      b. Create mock of this Interface
 *
 * 5. Solve Console.WriteLine
 *      a. Extract to Printer class
 *      b. Implement IPrinter
 *      c. Mock Iprinter
 *
 */