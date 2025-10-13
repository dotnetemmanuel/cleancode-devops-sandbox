namespace CalculatorApp.Tests;

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
