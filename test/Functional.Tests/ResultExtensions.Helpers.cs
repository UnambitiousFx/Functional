namespace UnambitiousFx.Functional.Tests;

public sealed partial class ResultExtensions
{
    private class Person
    {
        public string Name { get; set; } = "";
        public Address? Address { get; set; }
        public int Age { get; set; }
    }

    private class Address
    {
        public string City { get; set; } = "";
    }
}
