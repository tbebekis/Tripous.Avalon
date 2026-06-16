namespace Tripous.Tests;

public class IdTests
{
    [Fact]
    public void ImplicitInt_ReturnsShortValue()
    {
        ID Id = (short)5;

        int Result = Id;

        Assert.Equal(5, Result);
    }
    [Fact]
    public void ImplicitShort_ReturnsIntValue()
    {
        ID Id = 5;

        short Result = Id;

        Assert.Equal((short)5, Result);
    }
    [Fact]
    public void AreEqual_ReturnsTrueForNumericStringAndInt()
    {
        Assert.True(ID.AreEqual("5", 5));
    }
    [Fact]
    public void Equals_ReturnsTrueForNumericStringAndIntId()
    {
        ID Left = "5";
        ID Right = 5;

        Assert.Equal(Left, Right);
    }
    [Fact]
    public void GetHashCode_ReturnsSameHashForEqualNumericIds()
    {
        ID Left = "5";
        ID Right = 5;

        Assert.Equal(Left.GetHashCode(), Right.GetHashCode());
    }
}
