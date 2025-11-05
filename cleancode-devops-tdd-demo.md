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

Perfekt! Här kommer två nya exempel som bygger vidare på **IceCreamMachine**-temat. De följer exakt samma TDD-struktur som i originalet: **RED → GREEN → REFACTOR**, med tydliga kodblock och kommentarer. Du kan använda dem direkt i ditt manus eller demo.

---

## 🍦 Pris per glasskula

### 🧱 Syfte
Lägg till en metod `GetPrice(string size)` som returnerar priset för en glass baserat på storlek. Priset beräknas som antal kulor × 10 kr.

---

### 🧪 Steg 1: Skriv första testet (RED)

// `IceCreamMachine.Tests/IceCreamMachineTests.cs`
```csharp
[Fact]
public void GetPrice_ShouldReturn50_ForXL()
{
    var machine = new IceCreamMachine();
    var price = machine.GetPrice("XL");

    Assert.Equal(50, price);
}
```

Testet misslyckas – metoden `GetPrice` finns inte än.

---

### 🟢 Steg 2: Skriv minsta möjliga kod (GREEN)

// `IceCreamMachine/IceCreamMachine.cs`
```csharp
public int GetPrice(string size)
{
    return 50; // hårdkodat för att få testet att gå igenom
}
```

Testet går igenom, men koden är inte generell.

---

### 🟡 Steg 3: Refaktorera (REFACTOR)

// `IceCreamMachine/IceCreamMachine.cs`
```csharp
public int GetPrice(string size)
{
    var scoops = GetScoops(size);
    return scoops * 10;
}
```

Nu återanvänds `GetScoops`, vilket gör koden DRY och lätt att underhålla.

---

### 🧪 Steg 4: Lägg till fler tester
```csharp
[Theory]
[InlineData("S", 10)]
[InlineData("M", 30)]
[InlineData("L", 40)]
[InlineData("XL", 50)]
[InlineData("XXL", 0)] // ogiltig storlek
public void GetPrice_ShouldReturnCorrectPrice(string size, int expected)
{
    var machine = new IceCreamMachine();
    var price = machine.GetPrice(size);

    Assert.Equal(expected, price);
}
```

---

## 🍧 Är storleken giltig?

### 🧱 Syfte
Lägg till en metod `IsValidSize(string size)` som returnerar `true` om storleken är giltig, annars `false`.

---

### 🧪 Steg 1: Skriv första testet (RED)

// `IceCreamMachine.Tests/IceCreamMachineTests.cs`
```csharp
[Fact]
public void IsValidSize_ShouldReturnTrue_ForM()
{
    var machine = new IceCreamMachine();
    var result = machine.IsValidSize("M");

    Assert.True(result);
}
```

Testet misslyckas – metoden finns inte.

---

### 🟢 Steg 2: Skriv minsta möjliga kod (GREEN)

// `IceCreamMachine/IceCreamMachine.cs`
```csharp
public bool IsValidSize(string size)
{
    return true; // hårdkodat
}
```

---

### 🟡 Steg 3: Refaktorera (REFACTOR)

// `IceCreamMachine/IceCreamMachine.cs`
```csharp
public bool IsValidSize(string size)
{
    return scoopTable.ContainsKey(size);
}
```

Nu används den befintliga `scoopTable`, vilket gör metoden robust och konsekvent.

---

### 🧪 Steg 4: Lägg till fler tester
```csharp
[Theory]
[InlineData("S", true)]
[InlineData("M", true)]
[InlineData("L", true)]
[InlineData("XL", true)]
[InlineData("XXL", false)]
[InlineData("", false)]
[InlineData(null, false)]
public void IsValidSize_ShouldReturnExpectedResult(string size, bool expected)
{
    var machine = new IceCreamMachine();
    var result = machine.IsValidSize(size);

    Assert.Equal(expected, result);
}
```

---

Vill du att vi bygger vidare med t.ex. smaker, toppings eller kampanjer? Jag kan ta fram fler exempel som passar in i samma struktur.
