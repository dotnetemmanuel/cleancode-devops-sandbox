using EmployeeAccessDemo.Models;
using EmployeeAccessDemo.TestData;

namespace EmployeeAccessDemo.Tests;

public class AccessServiceTests
{
    private readonly AccessService _sut = new AccessService();

    [Theory]
    [ClassData(typeof(EmployeeAccessTestData))]
    public void FromClass_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
    {
        //Arrange is done in ClassData

        //Act
        var actual = _sut.CanAccessRestrictedArea(employee);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetAccessCases))]
    public void FromMember_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
    {
        //Arrange is done in MemberData (GetAccessCases())

        //Act
        var actual = _sut.CanAccessRestrictedArea(employee);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(EmployeeAccessJsonTestData))]
    public void FromJson_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
    {
        //Arrange is done in ClassData via load av data from json (simulated database)
        
        //Act
        var actual = _sut.CanAccessRestrictedArea(employee);

        //Assert
        Assert.Equal(expected, actual);
    }

    public static IEnumerable<object[]> GetAccessCases()
    {
        yield return new object[] { new Employee { Name = "Anna", Role = "Manager", IsClockedIn = true }, true };
        yield return new object[] { new Employee { Name = "Erik", Role = "Staff", IsClockedIn = true }, false };
        yield return new object[] { new Employee { Name = "Lisa", Role = "Manager", IsClockedIn = false }, false };
    }
}