using EmployeeAccessDemo.Models;

namespace EmployeeAccessDemo;

public class AccessService
{
    public bool CanAccessRestrictedArea(Employee employee)
    {
        return employee.Role == "Manager" && employee.IsClockedIn;
    }
}