using Microsoft.AspNetCore.Routing;

namespace StudentCI.Tests;

public class UnitTest1
{
    Calculator calculator = new Calculator();

    [Fact]
    public void test_additon()
    {
        //arrange
        int num1 = 100;
        int num2 = 250;
        var result = calculator.Add(num1, num2);
        Assert.Equal(result, (num1 + num2));
    }

    [Fact]
    public void test_substraction()
    {
        //arrange
        int num1 = 400;
        int num2 = 250;
        var result = calculator.substract(num1, num2);
        Assert.Equal(result, (num1 - num2));
    }
}