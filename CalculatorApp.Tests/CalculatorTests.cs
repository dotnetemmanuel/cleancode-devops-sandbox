namespace CalculatorApp.Tests;

public class CalculatorTests
{
    private readonly Calculator sut = new();

    [Fact]
    public void Add_TwoPlusTwo_ReturnsFour()
    {
        //Arrange
        int a = 2;
        int b = 2;
        int expected = 4;

        //Act
        int actual = sut.Add(a, b);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Subtract_FiveMinusThree_ReturnsTwo()
    {
        //Arrange
        int a = 5;
        int b = 3;
        int expected = 2;

        //Act
        int actual = sut.Subtract(a, b);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Multiply_ByZero_ReturnsZero()
    {
        //Arrange
        int a = 0;
        int b = 5;
        int expected = 0;

        //Act
        int actual = sut.Multiply(a, b);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Multiply_TwoByThree_ReturnsSix()
    {
        //Arrange
        int a = 2;
        int b = 3;
        int expected = 6;

        //Act
        int actual = sut.Multiply(a, b);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Divide_SixDividedByTwo_ReturnsThree()
    {
        //Arrange
        int a = 6;
        int b = 2;
        double expected = 3;

        //Act
        double actual = sut.Divide(a, b);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        //Arrange
        int a = 5;
        int b = 0;

        //Act & Assert
        Assert.Throws<DivideByZeroException>(() => sut.Divide(a, b));
    }
}