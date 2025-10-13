namespace CalculatorApp.Tests;

public class CalculatorTests
{
    [Fact]
    public void Add_TwoPlusTwo_ReturnsFour()
    {
        //Arrange
        var sut = new Calculator();
        
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
        var sut = new Calculator();
        
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
        var sut = new Calculator();

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
        var sut = new Calculator();

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
        var sut = new Calculator();

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
        var sut = new Calculator();

        int a = 5;
        int b = 0;

        //Act & Assert
        Assert.Throws<DivideByZeroException>(() => sut.Divide(a, b));
    }

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
}