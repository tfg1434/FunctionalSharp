namespace FunctionalSharp.Tests; 

public class UnitType {
    [Fact]
    public void Unit_OnlyOne() {
        Assert.Equal(new Unit(), Unit());
    }
}