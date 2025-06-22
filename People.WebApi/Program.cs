using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using People.Model;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/people", () =>
{
    var people = new List<Person>()
    {
        new Person()
        {
            Id = Guid.NewGuid(),
            LastName = "Smith",
            FirstName = "John"
        },
        new Person()
        {
            Id = Guid.NewGuid(),
            LastName = "Doe",
            FirstName = "Jane"
        }
    };
    return people;
});

await app.RunAsync();