- 📦 `DataValidationLib` – klassbiblioteket med vår logik
- 🧪 `DataValidationLib.Tests` – testprojektet med enhetstester

---

## 📦 Projekt: `DataValidationLib`

### Fil: `Validator.cs`

```csharp
namespace DataValidationLib;

public static class Validator
{
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(ch => "!@#$%^&*()_+-=[]{}|;:'\",.<>?/".Contains(ch))) return false;

        return true;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static bool IsPalindrome(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;

        var cleaned = new string(input
            .Where(char.IsLetterOrDigit)
            .Select(char.ToLower)
            .ToArray());

        var reversed = new string(cleaned.Reverse().ToArray());
        return cleaned == reversed;
    }
}
```

---

## 🧪 Projekt: `DataValidationLib.Tests`

### Fil: `ValidatorTests.cs`

```csharp
using DataValidationLib;
using Xunit;

namespace DataValidationLib.Tests;

public class ValidatorTests
{
    [Theory]
    // Happy path – uppfyller alla krav: längd, versal, siffra, specialtecken
    [InlineData("Password1!", true)]

    // Unhappy path – för kort, saknar flera krav
    [InlineData("pass", false)]

    // Unhappy path – saknar specialtecken
    [InlineData("PASSWORD123", false)]

    // Unhappy path – saknar versaler och siffror
    [InlineData("password!", false)]

    // Unhappy path – saknar siffror
    [InlineData("Password!", false)]

    // Unhappy path – tom sträng
    [InlineData("", false)]
    public void IsValidPassword_ShouldValidateCorrectly(string password, bool expected)
    {
        var result = Validator.IsValidPassword(password);
        Assert.Equal(expected, result);
    }

    [Theory]
    // Happy path – korrekt e-postformat
    [InlineData("test@example.com", true)]

    // Happy path – giltig e-post med subdomän
    [InlineData("user.name@domain.co.uk", true)]

    // Unhappy path – saknar domän efter @
    [InlineData("invalidemail@", false)]

    // Unhappy path – saknar toppdomän (t.ex. .com)
    [InlineData("invalid@domain", false)]

    // Unhappy path – tom sträng
    [InlineData("", false)]

    // Unhappy path – bara whitespace
    [InlineData("   ", false)]
    public void IsValidEmail_ShouldValidateCorrectly(string email, bool expected)
    {
        var result = Validator.IsValidEmail(email);
        Assert.Equal(expected, result);
    }

    [Theory]
    // Happy path – palindrom med blandade versaler
    [InlineData("Anna", true)]

    // Happy path – palindrom med specialtecken och mellanslag
    [InlineData("A man, a plan, a canal: Panama", true)]

    // Unhappy path – inte samma fram och bak
    [InlineData("Hello", false)]

    // Unhappy path – tom sträng
    [InlineData("", false)]

    // Happy path – numerisk palindrom
    [InlineData("12321", true)]

    // Happy path – palindrom med mellanslag och versaler
    [InlineData("Was it a car or a cat I saw?", true)]
    public void IsPalindrome_ShouldDetectCorrectly(string input, bool expected)
    {
        var result = Validator.IsPalindrome(input);
        Assert.Equal(expected, result);
    }
}
```

---
