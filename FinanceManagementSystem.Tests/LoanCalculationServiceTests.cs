using FinanceManagementSystem.Services;

namespace FinanceManagementSystem.Tests;

public class LoanCalculationServiceTests
{
    private readonly LoanCalculationService _service = new();

    [Fact]
    public void CalculateLoan_SimpleInterest_ReturnsCorrectAmount()
    {
        // Arrange
        decimal principal = 10000m;
        decimal rate = 12m;
        int tenure = 12;

        // Act
        var result = _service.CalculateLoan(
            principal, rate, "Simple", tenure);

        // Assert
        Assert.Equal(1200m, result.TotalInterest);
        Assert.Equal(11200m, result.TotalPayable);
    }

    [Fact]
    public void CalculateLoan_CompoundInterest_ReturnsCorrectAmount()
    {
        // Arrange
        decimal principal = 10000m;
        decimal rate = 12m;
        int tenure = 8;

        // Act
        var result = _service.CalculateLoan(
            principal, rate, "Compound", tenure);

        // Assert
        Assert.Equal(828.57m, result.TotalInterest);
        Assert.Equal(10828.57m, result.TotalPayable);
    }

    [Fact]
    public void CalculateLoan_ZeroPrincipal_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.CalculateLoan(
                0m, 12m, "Simple", 12));
    }

    [Fact]
    public void CalculateLoan_NegativeInterestRate_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.CalculateLoan(
                10000m, -5m, "Simple", 12));
    }

    [Fact]
    public void CalculateLoan_ZeroTenure_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.CalculateLoan(
                10000m, 12m, "Simple", 0));
    }

    [Fact]
    public void CalculateLoan_InvalidInterestType_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() =>
            _service.CalculateLoan(
                10000m, 12m, "Invalid", 12));
    }
}
