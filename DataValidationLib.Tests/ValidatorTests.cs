namespace DataValidationLib.Tests;

public class ValidatorTests
{
    [Theory]    
    [InlineData("Password1!", true)] // Happy path – uppfyller alla krav: längd, versal, siffra, specialtecken
    [InlineData("pass", false)] // Unhappy path – för kort, saknar flera krav
    [InlineData("PASSWORD123", false)] // Unhappy path – saknar specialtecken
    [InlineData("password!", false)] // Unhappy path – saknar versaler och siffror
    [InlineData("Password!", false)]// Unhappy path – saknar siffror
    [InlineData("", false)] // Unhappy path – tom sträng
    public void IsValidPassword_ShouldValidateCorrectly(string password, bool expected)
    {
        var result = Validator.IsValidPassword(password);
        Assert.Equal(expected, result);
    }

    [Theory]   
    [InlineData("test@example.com", true)] // Happy path – korrekt e-postformat
    [InlineData("user.name@domain.co.uk", true)] // Happy path – giltig e-post med subdomän
    [InlineData("invalidemail@", false)] // Unhappy path – saknar domän efter @
    [InlineData("", false)] // Unhappy path – tom sträng
    [InlineData("   ", false)] // Unhappy path – bara whitespace
    public void IsValidEmail_ShouldValidateCorrectly(string email, bool expected)
    {
        var result = Validator.IsValidEmail(email);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Anna", true)] // Happy path – palindrom med blandade versaler
    [InlineData("A man, a plan, a canal: Panama", true)] // Happy path – palindrom med specialtecken och mellanslag
    [InlineData("Hello", false)] // Unhappy path – inte samma fram och bak
    [InlineData("", false)] // Unhappy path – tom sträng
    [InlineData("12321", true)] // Happy path – numerisk palindrom
    [InlineData("Was it a car or a cat I saw?", true)] // Happy path – palindrom med mellanslag och versaler
    [InlineData("kayak", true)] // Happy path
    public void IsPalindrome_ShouldDetectCorrectly(string input, bool expected)
    {
        var result = Validator.IsPalindrome(input);
        Assert.Equal(expected, result);
    }
}