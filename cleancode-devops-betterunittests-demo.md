# 🎬 **Demo-manus: Bättre enhetstester med Calculator**

## Demo 1 - Använd samma `sut` för alla tester

Omfaktorera koden för `CalculatorApp.Tests` så att `sut`återanvänds enligt följande

> injicera det som ett readonly privatfält

>> `private readonly Calculator _sut = new Calculator();`

> använd sedan denna `sut` i alla tester

```csharp
[Fact]
public void Add_ReturnsSum() {
    var result = _sut.Add(2, 2);
    Assert.Equal(4, result);
}
```

### ⚠️ Återanvänd it samma `sut` när klassen har någon form av tillstånd (state)
T.ex:
```csharp
public class Calculator {
    private int total = 0;
    public int Add(int a) => total += a;
}
```
> …då skulle delad instans kunna påverka testresultat.

Här har Calculator ett fält total som:
  - Börjar på 0
  - Ökar varje gång Add() anropas
  - Påverkar resultatet nästa gång du anropar metoden


I sådana fall bör man skapa en ny instans per test.

## Demo 2 - Use `IClassFixture`

### 🧮 Step 1: `AdvancedCalculator.cs` Class with Propertie

```csharp
public class AdvancedCalculator {
    public string Owner { get; set; }
    public string Model { get; set; }
    public bool IsScientific { get; set; }

    public int Add(int a, int b) => a + b;
}
```

### 🧰 Step 2: `AdvancedCalculatorFixture`

```csharp
public class AdvancedCalculatorFixture {
    public AdvancedCalculator Calculator { get; }

    public AdvancedCalculatorFixture() {
        Calculator = new AdvancedCalculator {
            Owner = "Daniel",
            Model = "TI-84 Plus",
            IsScientific = true
        };
    }
}
```

### 🧪 Step 3: Fun and Simple Tests

```csharp
public class AdvancedCalculatorTests : IClassFixture<AdvancedCalculatorFixture> {
    private readonly AdvancedCalculatorFixture _calculatorFixture;
    private readonly AdvancedCalculator _sut;

    public AdvancedCalculatorTests(AdvancedCalculatorFixture calculatorFixture) {
        _calculatorFixture = calculatorFixture;
        _sut = _calculatorFixture.Calculator;
    }

    [Fact]
    public void Owner_IsDaniel() => Assert.Equal("Daniel", _sut.Owner);

    [Fact]
    public void Model_ContainsTI() => Assert.Contains("TI", _sut.Model);

    [Fact]
    public void IsScientific_IsTrue() => Assert.True(_sut.IsScientific);

    [Fact]
    public void Add_TwoPlusThree_ReturnsFive() => Assert.Equal(5, _sut.Add(2, 3));
}
```

## Demo 3 - Use a `ICollectionFixture`

### Step 1 - In `CalculatorApp.Tests`, create an `AdvancedCalculatorCollection` class
Explain that ICollectionFixture is built in Xunit. Explain that we set the name by defining the collection. It can also be replaced by a string, but it is always preferable to avoid magic strings.

```csharp
[CollectionDefinition(nameof(AdvancedCalculatorCollection))]
public class AdvancedCalculatorCollection : ICollectionFixture<AdvancedCalculatorFixture>
{

}
```

### Step 2 - Create two new test classes for the purpose of Demo
Explain that we tell for the test suite which collection we want to use in the tests. Can also be replaced by a string but same as before.

`ModelTests.cs`
```csharp
[Collection(nameof(AdvancedCalculatorCollection))]
public class ModelTests
{
    private readonly AdvancedCalculatorFixture _fixture;

    public ModelTests(AdvancedCalculatorFixture fixture) {
        _fixture = fixture;
    }

    [Fact]
    public void Model_ContainsTI() {
        // Arrange
        var expectedSubstring = "TI";

        // Act
        var actual = _fixture.Calculator.Model;

        // Assert
        Assert.Contains(expectedSubstring, actual);
    }

    [Fact]
    public void IsScientific_IsTrue() {
        // Arrange
        var expected = true;

        // Act
        var actual = _fixture.Calculator.IsScientific;

        // Assert
        Assert.True(actual);
    }
}
```

`OwnerTests`
```csharp
[Collection(nameof(AdvancedCalculatorCollection))]
public class OwnerTests {
    private readonly AdvancedCalculatorFixture _fixture;

    public OwnerTests(AdvancedCalculatorFixture fixture) {
        _fixture = fixture;
    }

    [Fact]
    public void Owner_IsDaniel() {
        // Arrange
        var expected = "Daniel";

        // Act
        var actual = _fixture.Calculator.Owner;

        // Assert
        Assert.Equal(expected, actual);
    }
}
```

#### 🧠 Den verkliga skillnaden: Scope och delning

**Utan** [Collection]

Varje testklass får sin egen instans av fixture. Även om flera klasser använder samma fixture-typ, skapar xUnit en ny instans per klass.

**Med** [Collection("...")] och ICollectionFixture<T>

  Alla testklasser som är markerade med samma [Collection("...")] delar en och samma instans av fixture. Det innebär:
  - Delad setup mellan klasser
  - Delat tillstånd (om fixture innehåller tillstånd)
  - Delad teardown (om det är implementerat)

✅ **Med** [Collection("...")] och ICollectionFixture<T>

  - Du definierar fixture en gång (t.ex. med Owner = "Daniel", Model = "TI-84 Plus", etc.).
  - Du annoterar flera testklasser med [Collection("Calculator collection")].
  - xUnit skapar en gemensam instans av fixture och injicerar den i alla dessa klasser.
  - Det innebär att alla testklasser använder samma konfigurerade calculator, utan att upprepa setup.

❌ **Utan** [Collection]

  - Du kan fortfarande injicera fixture i varje testklass med IClassFixture<T>.
  - Men xUnit kommer att skapa en ny instans av fixture för varje testklass.
  - Så om din fixture sätter Owner = "Daniel" i konstruktorn, kommer den setupen att upprepas — och varje testklass får sin egen kopia.

## Demo 4 - Datadrivna tester`

### Step 1 - Lägg till följande i `CalculatorTests`

```csharp
[Theory]
    [InlineData(3, 2, 5)]
    [InlineData(1, 2, 3)]
    [InlineData(57, 13, 70)]
    public void CanAdd(int a, int b, int sum)
    {
        //Arrange
        var sut = new Calculator();
        var expected = sum;

        //Act
        var actual = sut.Add(a, b);

        //Assert
        Assert.Equal(expected, actual);
    }
```

Diskutera poängen med det och hur det underlättar testning

Lägg till extra tester:

```csharp
public class AdvancedCalculatorTheoryTests
{
    [Theory]
    [InlineData("Daniel", true)]
    [InlineData("StudentCalc", false)]
    [InlineData("TI-84 Plus", true)]
    public void OwnerName_IsScientificFlagMatches(string owner, bool expectedScientific)
    {
        // Arrange
        var sut = new AdvancedCalculator
        {
            Owner = owner,
            IsScientific = expectedScientific
        };

        // Act
        var actual = sut.IsScientific;

        // Assert
        Assert.Equal(expectedScientific, actual);
    }
```

Få studenterna att reflektera över testet och hitta vad som egentligen inte har någon relevans i det eller spelar roll i resultatet
Varför används då "owner" propertyn i testet? => kunna instansiera vår class som har denna egenskap.

```csharp
[Theory]
    [InlineData("TI-84 Plus", "TI")]
    [InlineData("Casio FX-991", "Casio")]
    [InlineData("Sharp EL-W531", "Sharp")]
    public void Model_StartsWithExpectedPrefix(string model, string expectedPrefix)
    {
        // Arrange
        var sut = new AdvancedCalculator
        {
            Model = model
        };

        // Act
        var actual = sut.Model;

        // Assert
        Assert.StartsWith(expectedPrefix, actual);
    }
```

## Demo 5 - Delad Data

🧩 Scenario: Behörighet till ett begränsat område
Vi vill testa om en anställd får komma in i ett begränsat område. Regeln är:
Endast chefer som är inloggade får komma in.

### Steg 0 - Skapa en class library (nytt projekt)
`EmployeeAccessDemo.cs`

### ✅ Steg 1: Skapa modellen
📄 Models/Employee.cs

```csharp
public class Employee {
    public string Name { get; set; }
    public string Role { get; set; }
    public bool IsClockedIn { get; set; }
}
```

### ✅ Steg 2: Skapa en metod att testa
📄 Services/AccessService.cs

```csharp
public class AccessService {
    public bool CanAccessRestrictedArea(Employee employee) {
        return employee.Role == "Manager" && employee.IsClockedIn;
    }
}
```

### ✅ Steg 3: Skapa testdata med ClassData
📄 TestData/EmployeeAccessTestData.cs

```csharp
using System.Collections;
using System.Collections.Generic;

public class EmployeeAccessTestData : IEnumerable<object[]> {
    public IEnumerator<object[]> GetEnumerator() {
        yield return new object[] { new Employee { Name = "Anna", Role = "Manager", IsClockedIn = true }, true };
        yield return new object[] { new Employee { Name = "Erik", Role = "Staff", IsClockedIn = true }, false };
        yield return new object[] { new Employee { Name = "Lisa", Role = "Manager", IsClockedIn = false }, false };
        yield return new object[] { new Employee { Name = "Tom", Role = "Staff", IsClockedIn = false }, false };
    }

    // Explicit interface implementation
    // Your class already implements IEnumerable<object[]>, which is the generic version. But [ClassData] expects the class to also implement the non-generic IEnumerable interface.

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
```

### ✅ Steg 4: Skriv testmetoden med [Theory] och [ClassData]

Skapa ett testprojekt `EmployeeAcessDemo.Tests`

📄 `AccessServiceTests.cs`
```csharp
using Xunit;

public class AccessServiceTests {
    private readonly AccessService _sut = new AccessService();
    // Fråga varför inte tillgång till AccessService. => project reference.
    [Theory]
    [ClassData(typeof(EmployeeAccessTestData))]
    public void FromClass_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected) {
        // Arrange is done via test data

        // Act
        var actual = _sut.CanAccessRestrictedArea(employee);

        // Assert
        Assert.Equal(expected, actual);
    }
}
```

### ✅ Steg 5: Skriv testmetoden med [Theory] och [MemberData]

#### 1. Skapa testdata
📄 `AccessServiceTests.cs`

```csharp
public static IEnumerable<object[]> GetAccessCases()
{
    yield return new object[] { new Employee { Name = "Anna", Role = "Manager", IsClockedIn = true }, true };
    yield return new object[] { new Employee { Name = "Erik", Role = "Staff", IsClockedIn = true }, false };
    yield return new object[] { new Employee { Name = "Lisa", Role = "Manager", IsClockedIn = false }, false };
}
```

#### 2. Använd [MemberData] i testmetoden

```csharp
[Theory]
[MemberData(nameof(GetAccessCases))]
public void CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
{
    var sut = new AccessService();
    var actual = sut.CanAccessRestrictedArea(employee);
    Assert.Equal(expected, actual);
}
```

### Skillnad mellan ClassData och MemberData

| Egenskap             | `ClassData`                                 | `MemberData`                                     |
|----------------------|----------------------------------------------|--------------------------------------------------|
| Källa för testdata   | Extern klass som implementerar `IEnumerable<object[]>` | Metod eller egenskap (`public static`)           |
| Syntax               | `[ClassData(typeof(...))]`                   | `[MemberData(nameof(...))]`                      |
| Placering            | I separat klass                             | I testklassen eller annan klass (`MemberType`)   |
| Återanvändbarhet     | Hög – kan användas i flera testklasser       | Medel – oftast lokal till testklassen            |
| Flexibilitet         | Hög – kan ha konstruktorer och logik         | Medel – enklare att skriva, mindre logik         |
| Synlighet i testfil  | Låg – testdata ligger utanför testfilen      | Hög – testdata syns direkt i testfilen           |

### ✅ Steg 6 - med extern data

#### 🧾 1. Skapa JSON-fil med testfall

📄 `EmployeeAccessDemo/TestData/employee_access_cases.json`
```json
[
  {
    "employee": { "name": "Anna", "role": "Manager", "isClockedIn": true },
    "expected": true
  },
  {
    "employee": { "name": "Erik", "role": "Staff", "isClockedIn": true },
    "expected": false
  },
  {
    "employee": { "name": "Lisa", "role": "Manager", "isClockedIn": false },
    "expected": false
  },
  {
    "employee": { "name": "Tom", "role": "Staff", "isClockedIn": false },
    "expected": false
  }
]
```

⚠️
- Lägg detta till `EmployeeAccessDemo.Tests.csproj` så att filen kopieras till bin-mappen, vilket behövs då Xunit använder sig av den mappen för att köra testerna
- Se till att json filen har `Content`som BuildAction och `Copy always` som copy to output directory

```csharp
<ItemGroup>
        <None Include="..\EmployeeAccessDemo\TestData\employee_access_cases.json">
            <CopyToOutputDirectory>Always</CopyToOutputDirectory>
            <Link>employee_access_cases.json</Link>
        </None>
    </ItemGroup>
```

🔧 Glöm inte att sätta Copy to Output Directory till Copy if newer för filen.


#### 🧠 2. Skapa ny klass: EmployeeAccessJsonTestData
📄 `EmployeeAccessDemo/TestData/EmployeeAccessJsonTestData.cs`
```csharp
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using EmployeeAccessDemo.Models;

namespace EmployeeAccessDemo.TestData;

public class EmployeeAccessJsonTestData : IEnumerable<object[]>
{
  private const string FilePath = "TestData/employee_access_cases.json";

  public IEnumerator<object[]> GetEnumerator()
  {
      foreach (var testCase in LoadFromJson())
      {
          yield return new object[] { testCase.Employee, testCase.Expected };
      }
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  private static IEnumerable<EmployeeAccessCase> LoadFromJson()
      {
          var basePath = AppContext.BaseDirectory;
          var filePath = Path.Combine(basePath, "employee_access_cases.json");

          if (!File.Exists(filePath))
              throw new FileNotFoundException($"Testdatafil saknas: {filePath}");

          var json = File.ReadAllText(filePath);
          var cases = JsonSerializer.Deserialize<List<EmployeeAccessCase>>(json, new JsonSerializerOptions
          {
              PropertyNameCaseInsensitive = true
          });
          return cases ?? new List<EmployeeAccessCase>();
      }

  private class EmployeeAccessCase
  {
      public Employee Employee { get; set; }
      public bool Expected { get; set; }
  }
}
```


#### 🧪 3. Lägg till ny testmetod i testprojektet
📄 `EmployeeAccessDemo.Tests/AccessServiceTests.cs`
```csharp
using EmployeeAccessDemo.Models;
using EmployeeAccessDemo.Services;
using EmployeeAccessDemo.TestData;
using Xunit;

namespace EmployeeAccessDemo.Tests;

public class AccessServiceTests
{
  private readonly AccessService _sut = new AccessService();

  [Theory]
  [ClassData(typeof(EmployeeAccessJsonTestData))]
  public void FromJson_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
  {
      var actual = _sut.CanAccessRestrictedArea(employee);
      Assert.Equal(expected, actual);
  }
}
