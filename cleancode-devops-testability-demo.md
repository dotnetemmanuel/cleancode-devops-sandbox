# 🧪 Demo: Testbarhet – från kaos till kontroll
## 🎯 Syfte
Visa hur man identifierar och bryter ut svårtestade beroenden (som DateTime.Now, Console, Database) för att göra kod testbar med hjälp av gränssnitt, injektion, och mockning.

## 🧨 Projekt 1: TestabilityDemo.Original
### Syfte: Visa den svårtestade koden

#### 🛠️ Vad vi vill visa
Vi börjar med en typisk "kaos"-kod där all logik ligger i Main, och beroenden som tid, databas och konsol är hårdkodade. Den är svår att testa eftersom vi inte kan styra eller ersätta dessa beroenden.

### 📁 Skapa projektet
dotnet new console -n TestabilityDemo.Original

📄 Program.cs

// `TestabilityDemo.Original/Program.cs`

```csharp
var db = new Database();
var now = DateTime.Now;
var discount = (now.DayOfWeek == DayOfWeek.Friday) ? 25f : 0f;

Console.WriteLine("Vad vill du köpa?");
var item = Console.ReadLine();

var price = db.GetItemPrice(item);

Console.WriteLine("Hur många?");
var quantity = float.Parse(Console.ReadLine());

var total = price * quantity * (1 - (discount / 100));

Console.WriteLine($"""
    ********************************
    KVITTO
    {item}({price}) x {quantity}
    rabatt: {discount}%
    Total price: {total}:-
    ~~~~~~~~~~~~~~~~~~~~~
    {now}
    ********************************
    """);

public class Database
{
    public float GetItemPrice(string id) => new Random().Next(1, 10);
}
```

Problem:
- All logik i Main → svårt att testa
- Database är hårdkopplad → svårt att mocka
- DateTime.Now → svårt att styra i test
- Console.ReadLine / Console.WriteLine → svårt att fånga in/utdata

## 🧪 Projekt 2: TestabilityDemo.Core
### Syfte: Refaktorerad, testbar kod

#### 🛠️ Vad vi vill lösa
Vi bryter ut logiken från Main till en tjänstklass (ReceiptService) och ersätter beroenden med gränssnitt. Det gör att vi kan injicera olika implementationer – t.ex. mockar i test – och därmed testa logiken isolerat.


### 📁 Skapa projektet
dotnet new classlib -n TestabilityDemo.Core

📄 ReceiptService.cs

// `TestabilityDemo.Core/ReceiptService.cs`
```csharp
namespace TestabilityDemo.Core;

public class ReceiptService
{
    public void PrintReceipt(string item, float quantity, IClock clock, IDatabase db, IPrinter printer)
    {
        var now = clock.Now;
        var discount = (now.DayOfWeek == DayOfWeek.Friday) ? 25f : 0f;
        var price = db.GetItemPrice(item);
        var total = price * quantity * (1 - (discount / 100));

        printer.Print($"""
            ********************************
            KVITTO
            {item}({price}) x {quantity}
            rabatt: {discount}%
            Total price: {total}:-
            ~~~~~~~~~~~~~~~~~~~~~
            {now}
            ********************************
            """);
    }
}
```

💡Interfaces gör beroenden testbara

📄 Interfaces.cs

//`TestabilityDemo.Core/Interfaces.cs`
```csharp
namespace TestabilityDemo.Core;

public interface IClock
{
    DateTime Now { get; }
}

public interface IDatabase
{
    float GetItemPrice(string id);
}

public interface IPrinter
{
    void Print(string text);
}
```


📄 Implementations.cs

// `TestabilityDemo.Core/Implementations.cs`
```csharp
namespace TestabilityDemo.Core;

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
}

public class Database : IDatabase
{
    public float GetItemPrice(string id) => new Random().Next(1, 10);
}

public class ConsolePrinter : IPrinter
{
    public void Print(string text) => Console.WriteLine(text);
}
```

## 🧪 Projekt 3: TestabilityDemo.Tests
### Syfte: Enhetstester med mockade beroenden

#### 🛠️ Vad vi vill testa
Vi vill verifiera att logiken i ReceiptService fungerar korrekt – t.ex. att fredagsrabatten tillämpas – utan att bero på verklig tid, konsol eller databas. Därför mockar vi alla beroenden.


### 📁 Skapa projektet
dotnet new xunit -n TestabilityDemo.Tests
dotnet add TestabilityDemo.Tests/TestabilityDemo.Tests.csproj reference TestabilityDemo.Core/TestabilityDemo.Core.csproj

📄 ReceiptServiceTests.cs

// `TestabilityDemo.Tests/ReceiptServiceTests.cs`
```csharp
using TestabilityDemo.Core;
using Xunit;

public class FakeClock : IClock
{
    public DateTime Now { get; set; }
    //the FakeClock class to simulate a specific day of the week relative to the fixed reference date, which can be useful for testing scenarios that depend on the current day.
    public FakeClock(DayOfWeek day) => Now = new DateTime(2025, 10, 10).AddDays(day - DayOfWeek.Friday);
}

public class FakeDatabase : IDatabase
{
    private readonly float _price;
    public FakeDatabase(float price) => _price = price;
    public float GetItemPrice(string id) => _price;
}

public class TestPrinter : IPrinter
{
    // Using private set ensures only the class itself can change Output, preserving encapsulation and test integrity.
    public string Output { get; private set; } = "";
    public void Print(string text) => Output = text;
}

public class ReceiptServiceTests
{
    [Fact]
    public void ShouldApplyFridayDiscount()
    {
        var clock = new FakeClock(DayOfWeek.Friday);
        var db = new FakeDatabase(10f);
        var printer = new TestPrinter();

        var service = new ReceiptService();
        service.PrintReceipt("Banana", 2, clock, db, printer);

        Assert.Contains("rabatt: 25%", printer.Output);
        Assert.Contains("Total price: 15", printer.Output); // 10 * 2 * 0.75 = 15
    }

    //Add this after the first test is demoed.
    [Fact]
        public void ShouldNotApplyDiscountOnMonday()
        {
            // 🧪 Arrange => förklara att beroende på vilken data man vill använda så kan man flytta vissa arrange statements ut
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
```

🧭 Sammanfattning av filstruktur

```
TestabilityDemo.Original/
└── Program.cs

TestabilityDemo.Core/
├── ReceiptService.cs
├── Interfaces.cs
└── Implementations.cs

TestabilityDemo.Tests/
└── ReceiptServiceTests.cs
```

✅ Steg-för-steg-verifiering
1. Identifiera problem
- a. All kod i Main
✔️ Originalkoden ligger i Program.cs i TestabilityDemo.Original och visar problemet med all logik i Main.
- b. Database dependency
✔️ Database är hårdkodad i originalet. I refaktorn ersätts den med IDatabase och mockas i testprojektet.
- c. DateTime.Now
✔️ Ersatt med IClock och SystemClock. Mockas med FakeClock i testet.
- d. Console.WriteLine
✔️ Ersatt med IPrinter och ConsolePrinter. Mockas med TestPrinter.

2. Extract code to PrintReceipt
✔️ All logik flyttas till ReceiptService.PrintReceipt(...) i TestabilityDemo.Core/ReceiptService.cs.

3. Solve Database
- a. Adding interface
✔️ IDatabase finns i Interfaces.cs.
- b. Adding mock of interface
✔️ FakeDatabase används i ReceiptServiceTests.cs.

4. Solve DateTime.Now
- a. Add wrapper class and Interface
✔️ IClock och SystemClock finns i Interfaces.cs och Implementations.cs.
- b. Create mock of this Interface
✔️ FakeClock används i testet.

5. Solve Console.WriteLine
- a. Extract to Printer class
✔️ ConsolePrinter i Implementations.cs.
- b. Implement IPrinter
✔️ IPrinter finns i Interfaces.cs.
- c. Mock IPrinter
✔️ TestPrinter används i testet.

Vill du att jag lägger till en README-fil som dokumenterar dessa steg i projektet, så att studenterna kan följa dem som en övning? Jag kan också lägga till en interaktiv uppgift där de själva får mocka en ny beroende.
