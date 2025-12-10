using Xunit;

namespace MonadCraft.Tests;

public class UnitTests
{
    [Fact]
    public void Default_instances_are_equal()
    {
        var equal = Unit.Default == new Unit();
        Assert.True(equal);
    }

    [Fact]
    public void Inequality_operator_returns_false()
    {
        var notEqual = Unit.Default != new Unit();
        Assert.False(notEqual);
    }

    [Fact]
    public void ToString_returns_parens()
    {
        var text = Unit.Default.ToString();
        Assert.Equal("()", text);
    }

    [Fact]
    public void Hash_code_is_zero()
    {
        var hash = Unit.Default.GetHashCode();
        Assert.Equal(0, hash);
    }
}