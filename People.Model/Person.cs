using System;

namespace People.Model;

public class Person
{
    public Guid Id { get; set; }
    public string ShortId => Id.ToString("D")[0..8];
    public string LastName { get; set; }
    public string FirstName { get; set; }
    public string LastNameWithInitial => $"{LastName} {FirstName[0]}.";
}