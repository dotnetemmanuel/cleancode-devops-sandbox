# 🍨 GlassMaskin – TDD-demo

## 🧱 Syfte
Skapa en klass som returnerar antal glasskulor baserat på storlek.
Storlekar: "S", "M", "L", "XL" → antal kulor: 1, 3, 4, 5
All logik ska vara enhetstestad med xUnit.

### 📁 Steg 1: Skapa filerna
- Skapa ett nytt Test-projekt: IceCreamMachine.Tests
- Skapa ett nytt Class Library-projekt: IceCreamMachine
- Lägg till en projektreferens från IceCreamMachine.Tests till IceCreamMachine
- Installera xUnit och xUnit.runner.visualstudio via NuGet i testprojektet

### 🧪 Steg 2: Skriv första testet (RED)
// `IceCreamMachine.Tests/IceCreamMachineTests.cs`
```csharp
using Xunit;
using IceCreamMachine;

public class IceCreamMachineTests
{
    [Fact]
    public void GetScoops_ShouldReturnFive_ForXLSize()
    {
        var machine = new IceCreamMachine();
        var result = machine.GetScoops("XL");

        Assert.Equal(5, result);
    }
}
```

Testet förväntar sig att "XL" ger 5 kulor. Det kommer misslyckas —  varken klass eller metoden finns än.

### 🟢 Steg 3: Skriv minsta möjliga kod (GREEN)
// `IceCreamMachine/IceCreamMachine.cs`
```csharp
namespace IceCreamMachine;

public class IceCreamMachine
{
    public int GetScoops(string size)
    {
        return size == "XL" ? 5 : 0;
    }
}
```

Testet går igenom — även om koden är ful och hårdkodad.

### 🟡 Steg 4: Refaktorera (REFACTOR)
// `IceCreamMachine/IceCreamMachine.cs`
```csharp
namespace IceCreamMachine;

public class IceCreamMachine
{
    private readonly Dictionary<string, int> scoopTable = new()
    {
        { "S", 1 },
        { "M", 3 },
        { "L", 4 },
        { "XL", 5 }
    };

    public int GetScoops(string size)
    {
        return scoopTable.TryGetValue(size, out var scoops) ? scoops : 0;
    }
}
```

Nu är koden tydlig, utbyggbar och testvänlig.

### 🧪 Steg 5: Lägg till fler tester
```csharp
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
```

✅ Resultat
- Du har en testdriven implementation av en enkel glassmaskin
- Du har följt RED → GREEN → REFACTOR
- Du har undvikit magiska nummer genom att använda InlineData och Dictionary
- Du har en kodbas som är lätt att bygga vidare på (t.ex. smaker, toppings, kampanjkulor)
