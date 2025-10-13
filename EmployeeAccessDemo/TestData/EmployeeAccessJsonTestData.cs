﻿using System.Collections;
using System.Text.Json;
using EmployeeAccessDemo.Models;

namespace EmployeeAccessDemo.TestData;

public class EmployeeAccessJsonTestData : IEnumerable<object[]>
{
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