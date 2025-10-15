using TestabilityDemo.Core;

namespace TestabilityDemo.Tests;

public class FakeClock : IClock
{
    public DateTime Now { get; set; }

    public FakeClock(DayOfWeek day)
    {
        //the FakeClock class to simulate a specific day of the week relative to the fixed reference date, which can be useful for testing scenarios that depend on the current day.
        Now = new DateTime(2025, 10, 10).AddDays((day - DayOfWeek.Friday));
    }
}

public class FakeDatabase : IDatabase
{
    private readonly float _price;

    public FakeDatabase(float price) => _price = price;
    public float GetItemPrice(string item) => _price;
}

public class TestPrinter : IPrinter
{
    public string Output { get; private set; } = "";
    public void Print(string text) => Output = text;
}

public class ReceiptServiceTests
{
    [Fact]
    public void ShouldApplyFridayDiscount()
    {
        //Arrange
        var clock = new FakeClock(DayOfWeek.Friday);
        var db = new FakeDatabase(10f);
        var printer = new TestPrinter();
        var service = new ReceiptService();

        //Act
        service.PrintReceipt("Banana", 2, clock, db, printer);

        //Assert
        Assert.Contains("rabatt: 25%", printer.Output);
        Assert.Contains("Total price: 15", printer.Output); // 10 * 2 * 0.75 = 15
    }
    
    [Fact]
    public void ShouldNotApplyDiscountOnMonday()
    {
        // 🧪 Arrange
        var clock = new FakeClock(DayOfWeek.Monday);
        var db = new FakeDatabase(7f);
        var printer = new TestPrinter();
        var service = new ReceiptService();

        // 🧪 Act
        service.PrintReceipt("Apple", 2, clock, db, printer);

        // 🧪 Assert
        Assert.Contains("rabatt: 0%", printer.Output);
        Assert.Contains("Total price: 14", printer.Output); // 10 * 2 * 1.0 = 20
    }
}