using UnambitiousFx.Functional.Failures;

namespace UnambitiousFx.Functional.Tests.Failures;

public class ValidationErrorTests
{
    [Theory]
    [InlineData(null,                          null)]
    [InlineData("",                            null)]
    [InlineData("amount",                      "#/amount")]
    [InlineData("Amount",                      "#/Amount")]
    [InlineData("payload.movements[3].amount", "#/payload/movements/3/amount")]
    [InlineData("items[0][1]",                 "#/items/0/1")]
    [InlineData("[0].Name",                    "#/0/Name")]
    [InlineData("$.amount",                    "#/amount")]
    [InlineData("$",                           "#")]
    [InlineData("tags['a.b'].value",           "#/tags/a.b/value")]
    [InlineData("tags[\"a/b\"]",               "#/tags/a~1b")]
    [InlineData("a~b",                         "#/a~0b")]
    [InlineData("a/b",                         "#/a~1b")]
    [InlineData("map[]",                       "#/map/")]
    [InlineData("broken[3",                    "#/broken/%5B3")]
    [InlineData("tags['a]b']",                 "#/tags/a%5Db")]
    [InlineData("tags['it\\'s']",              "#/tags/it's")]
    [InlineData("tags['open",                  "#/tags/%5B'open")]
    [InlineData("['$'].amount",                "#/$/amount")]
    [InlineData("$foo",                        "#/$foo")]
    [InlineData("a%2Fb",                       "#/a%252Fb")]
    [InlineData("first name",                  "#/first%20name")]
    [InlineData("prénom",                      "#/pr%C3%A9nom")]
    [InlineData("emoji😀",                     "#/emoji%F0%9F%98%80")]
    [InlineData("#/profile/color",             "#/profile/color")]
    [InlineData("/profile/color",              "/profile/color")]
    public void Pointer_WithField_ReturnsJsonPointer(string? field, string? expected)
    {
        // Arrange (Given)
        var error = new ValidationError(field, "Invalid.");

        // Act (When)
        var pointer = error.Pointer;

        // Assert (Then)
        Assert.Equal(expected, pointer);
    }

    [Fact]
    public void Equals_SameFieldMessageAndCode_IsUnaffectedByPointer()
    {
        // Arrange (Given)
        var left  = new ValidationError("payload.amount", "Invalid.", "amount_negative");
        var right = new ValidationError("payload.amount", "Invalid.", "amount_negative");

        // Act (When)
        var equal = left == right;

        // Assert (Then)
        Assert.True(equal);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }
}
