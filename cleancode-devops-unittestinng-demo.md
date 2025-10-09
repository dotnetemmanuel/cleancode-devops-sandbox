# 🎬 **Demo-manus: Enhetstester i praktiken med Calculator**

---

## 🧨 Steg 1: Svårtestad kod – och varför den är problematisk
```
CleanCodeDevops.sln
├── CalculatorApp          ← Konsolapplikation (.NET)
│   └── Calculator.cs      ← Här lägger du din klass
└── CalculatorApp.Tests    ← Class Library (.NET)
    └── CalculatorTests.cs ← Här lägger du dina tester
```

`Calculator` -klassen skapas i CalculatorApp.


```csharp
using System;

public class Calculator
{
    public void Add(int a, int b)
    {
        Console.WriteLine($"Adding {a} + {b}");
    }

    public void Subtract(int a, int b)
    {
        Log($"Subtracting {a} - {b}");
    }

    public void Multiply(int a, int b)
    {
        if (a == 0 || b == 0)
        {
            throw new InvalidOperationException("Multiplication by zero is not allowed.");
        }
    }

    public void Divide(int a, int b)
    {
        if (b == 0)
        {
            Console.WriteLine("Division by zero attempted.");
        }
    }
}
```

### ❌ Varför är detta svårt att testa?

- **Sid-effekter**: `Console.WriteLine()` och `File.AppendAllText()` gör att testresultat beror på I/O.
- **Undantag och specialfall**: Multiplikation kastar undantag vid noll, vilket är ologiskt och bryter förväntningar.
- **Inget "System Under Test" (SUT)**: Klassen är inte isolerad – den pratar med omvärlden.
- **Ingen separation av ansvar**: Logik och presentation blandas.

🗣️ **Demo-kommentar**:
_"Den här koden är som att försöka testa en brödrost medan någon håller på att bre mackor i den – det är stökigt, oförutsägbart och svårt att isolera vad som faktiskt testas."_

---

## ✅ Steg 2: Refaktorering – Gör koden testbar

```csharp
public class Calculator
{
    public int Add(int a, int b) => a + b;

    public int Subtract(int a, int b) => a - b;

    public int Multiply(int a, int b) => a * b;

    public double Divide(int a, int b)
    {
        if (b == 0) throw new DivideByZeroException("Division by zero is not allowed.");
        return (double)a / b;
    }
}
```

### ✔️ Varför är detta testbart?

- **Ren logik**: Inga sid-effekter, inga beroenden.
- **Förutsägbart beteende**: Multiplikation tillåter noll, division kastar tydligt undantag.
- **Enkel att mocka, isolera och testa**.

🗣️ **Demo-kommentar**:
_"Nu har vi en ren, deterministisk klass. Den gör exakt vad den ska – inget mer, inget mindre. Det är som att testa en miniräknare utan att behöva bry sig om batterier eller display."_

---

## 🧪 Steg 3: Skriv enhetstester med xUnit

Installera dessa endast i :
- xunit
- xunit.runner.visualstudio
- Microsoft.NET.Test.Sdk

```csharp
using Xunit;

public class CalculatorTests
{
    private readonly Calculator sut = new Calculator();

    [Fact]
    public void Add_TwoPlusTwo_ReturnsFour()
    {
        // Arrange – förbered testdata och objekt
        int a = 2;
        int b = 2;
        int expected = 4;

        // Act – utför operationen som ska testas
        int actual = sut.Add(a, b);

        // Assert – verifiera att resultatet är som förväntat
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Subtract_FiveMinusThree_ReturnsTwo()
    {
        // Arrange
        int a = 5;
        int b = 3;
        int expected = 2;

        // Act
        int actual = sut.Subtract(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Multiply_ThreeTimesFour_ReturnsTwelve()
    {
        // Arrange
        int a = 3;
        int b = 4;
        int expected = 12;

        // Act
        int actual = sut.Multiply(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Multiply_ByZero_ReturnsZero()
    {
        // Arrange
        int a = 0;
        int b = 5;
        int expected = 0;

        // Act
        int actual = sut.Multiply(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Divide_SixDividedByTwo_ReturnsThree()
    {
        // Arrange
        int a = 6;
        int b = 2;
        double expected = 3.0;

        // Act
        double actual = sut.Divide(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        int a = 5;
        int b = 0;

        // Act & Assert – vi förväntar oss ett undantag
        Assert.Throws<DivideByZeroException>(() => sut.Divide(a, b));
    }
}
```

🗣️ **Demo-kommentar**:
_"Här ser vi Arrange-Act-Assert i praktiken. Vi förbereder testet, kör det, och kollar att resultatet är som förväntat. Och vi testar både vanliga fall och edge cases – som division med noll."_

---

# 🧵 Avslutning: Vad du kan lyfta i din demo

- **Skillnaden mellan testbar och svårtestad kod** – visa båda!
- **AAA-mönstret** – pedagogiskt och tydligt.
- **xUnit i praktiken** – enkel syntax, kraftfullt resultat.
- **Edge cases och undantag** – visar att du tänker som en utvecklare.

# Testing other types and different assertions

---

### 🧮 Numbers

```csharp
public class MathService {
    public double Add(double a, double b) => a + b;
    public double GetPi() => 3.14159;
}

public class MathServiceTests {
    // Assert.Equal(expected, actual);
    [Fact]
    public void Add_ReturnsSum() {
        var sut = new MathService();
        var expected = 5;
        var actual = sut.Add(2, 3);
        Assert.Equal(expected, actual);
    }

    // Assert.Equal(expected, actual, precision);
    [Fact]
    public void Pi_IsCloseEnough() {
        var sut = new MathService();
        var expected = 3.14;
        var actual = sut.GetPi();
        Assert.Equal(expected, actual, 2);
    }

    // Assert.InRange(actual, min, max);
    [Fact]
    public void Sum_IsInRange() {
        var sut = new MathService();
        var actual = sut.Add(2, 3);
        Assert.InRange(actual, 4, 6);
    }
}
```

---

### 🔤 Strings

```csharp
public class StringService {
    public string Greet() => "Hello World";
    public string Echo(string input) => input;
}

public class StringServiceTests {
    // Assert.Equal(expected, actual)
    [Fact]
    public void Greet_IsExact() {
        var sut = new StringService();
        var expected = "Hello World";
        var actual = sut.Greet();
        Assert.Equal(expected, actual);
    }

    // Assert.Equal(expected, actual, ignoreCase: true);
    [Fact]
    public void Echo_IgnoresCase() {
        var sut = new StringService();
        var actual = sut.Echo("TEST");
        Assert.Equal("test", actual, ignoreCase: true);
    }

    // Assert.Contains(expected, actual);
    [Fact]
    public void Greet_ContainsHello() {
        var sut = new StringService();
        var actual = sut.Greet();
        Assert.Contains("Hello", actual);
    }

    // Assert.StartsWith(expected, actual);
    [Fact]
    public void Greet_StartsWithHello() {
        var sut = new StringService();
        var actual = sut.Greet();
        Assert.StartsWith("Hello", actual);
    }

    // Assert.Matches(pattern, actual);
    [Fact]
    public void Echo_MatchesPattern() {
        var sut = new StringService();
        var actual = sut.Echo("Test123");
        Assert.Matches(@"^\w+$", actual);
    }
}
```

---

### 📚 Collections

```csharp
public class CollectionService {
    public List<string> GetItems() => new() { "one", "two", "three" };
    public List<string> GetEmpty() => new();
}

public class CollectionServiceTests {
    // Assert.Contains(expected, collection);
    [Fact]
    public void Items_ContainTwo() {
        var sut = new CollectionService();
        var actual = sut.GetItems();
        Assert.Contains("two", actual);
    }

    // Assert.Empty(collection);
    [Fact]
    public void Empty_IsEmpty() {
        var sut = new CollectionService();
        var actual = sut.GetEmpty();
        Assert.Empty(actual);
    }

    // Assert.All(list, item => Assert.True(item.IsAwesome()));
    [Fact]
    public void AllItems_AreLowercase() {
        var sut = new CollectionService();
        var actual = sut.GetItems();
        Assert.All(actual, item => Assert.True(item == item.ToLower()));
    }
}
```

---

### 🧪 Miscellaneous

```csharp
public class ObjectService {
    public object GetNull() => null;
    public object GetSame() => new object();
    public void Crash() => throw new InvalidOperationException();
}

public class ObjectServiceTests {
    // Assert.Null(obj);
    [Fact]
    public void Null_IsNull() {
        var sut = new ObjectService();
        var actual = sut.GetNull();
        Assert.Null(actual);
    }

    // Assert.True(condition);
    [Fact]
    public void True_IsTrue() {
        Assert.True(1 + 1 == 2);
    }

    // Assert.Same(expected, actual);
    [Fact]
    public void Same_IsSame() {
        var sut = new ObjectService();
        var expected = sut.GetSame();
        var actual = expected;
        Assert.Same(expected, actual);
    }

    // Assert.IsType<T>(obj);
    [Fact]
    public void Type_IsInt() {
        var actual = 123;
        Assert.IsType<int>(actual);
    }

    // Assert.Throws<SpectacularCrashException>(() => sut.Method());
    [Fact]
    public void Throws_Exception() {
        var sut = new ObjectService();
        Assert.Throws<InvalidOperationException>(() => sut.Crash());
    }
}
```
---

❌ Varför den ursprungliga Calculator-klassen inte var testbar
- Blandning av logik och presentation
– Metoderna innehöll Console.WriteLine, vilket kopplar affärslogik direkt till UI. Det gör att du inte kan testa metoden utan att fånga konsolutskrift.
- Saknade return-värden
– Istället för att returnera resultatet av en beräkning, skrev metoden ut det. Tester behöver kunna jämföra värden, inte läsa från konsolen.
- Ingen separation av ansvar (SRP)
– Klassen gjorde flera saker: beräknade, skrev ut, kanske även tog input. Det bryter mot principen om enhetlig ansvarsfördelning och försvårar testning.
- Ingen determinism
– Om metoden använder Console.ReadLine() eller beroenden som inte är kontrollerade, blir testresultaten oförutsägbara.
- Ingen isolering
– Utan att isolera logiken från I/O är det svårt att skriva enhetstester som bara testar beräkningen.

✅ Hur den nya Calculator-koden blev testbar
- Ren logik, inga Console.WriteLine
– Metoden Add(int a, int b) returnerar ett värde direkt, vilket gör den enkel att testa.
- Tydlig input och output
– Metoden tar parametrar och returnerar ett resultat — perfekt för Assert.
- System Under Test (SUT) är isolerad
– Calculator är en ren klass utan beroenden, vilket gör den enkel att instansiera och testa.
- AAA-struktur i testet
– Testet följer Arrange–Act–Assert, vilket gör det lätt att förstå och felsöka.
- Ingen beroende på miljö eller användare
– Testet körs helt utan att behöva interagera med konsolen eller externa resurser.
