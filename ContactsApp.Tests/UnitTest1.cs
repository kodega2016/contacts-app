namespace ContactsApp.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {   
        // Arrange
        MyMath math = new MyMath();
        int input1 = 10,input2 = 5;
        int expected = 15;
        // Act
        int result=math.Add(input1, input2);
        // Assert
        Assert.Equal(expected, result);

    }
}
