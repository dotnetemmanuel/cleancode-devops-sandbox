namespace CalculatorApp.Tests;

public class AdvancedCalculatorTests : IClassFixture<AdvancedCalculatorFixture>
{
    private readonly AdvancedCalculatorFixture _calculatorFixture;
    private readonly AdvancedCalculator _sut;

    public AdvancedCalculatorTests(AdvancedCalculatorFixture calculatorFixture)
    {
        _calculatorFixture = calculatorFixture;
        _sut = _calculatorFixture.Calculator;
    }

    [Fact]
    public void Owner_IsDaniel()
    {
        //Arrange
        var expected = "Daniel";

        //Act
        var actual = _sut.Owner;
        
        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Model_ContainsTI()
    {
        //Arrange
        var expectedSubstring = "TI";

        //Act
        var actual = _sut.Model;

        //Assert
        Assert.Contains(expectedSubstring, actual);
    }

    [Fact]
    public void IsScientific_True()
    {
        //Arrange
        var expected = true;

        //Act
        var actual = _sut.IsScientific;

        //Assert
        Assert.True(actual);
        // Assert.Equal(expected, actual);
        // Assert.IsType<bool>(actual);
    }
    
    [Fact]
    public void Add_TwoPlusThree_ReturnsFive()
    {
        // Arrange
        int a = 2;
        int b = 3;
        int expected = 5;

        // Act
        int actual = _sut.Add(a, b);

        // Assert
        Assert.Equal(expected, actual);
    }

}