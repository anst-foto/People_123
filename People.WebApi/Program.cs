using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using People.Model;
using People.WebApi;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PeopleContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();


app.UseHttpsRedirection();

app.MapGet("/people", async (PeopleContext db) => 
    await db.People.ToArrayAsync());

app.MapGet("/people/{id:guid}", async (PeopleContext db, Guid id) => 
    await db.People.SingleOrDefaultAsync(p => p.Id == id));

app.MapGet("people/{name}", async (PeopleContext db, string name) => 
    await db.People.Where(p => p.LastName.ToLower() == name.ToLower() || 
                               p.FirstName.ToLower() == name.ToLower())
        .ToArrayAsync());

app.MapPost("/people", async (PeopleContext db, Person person) =>
{
    await db.People.AddAsync(person);
    await db.SaveChangesAsync();
});

app.MapPut("/people/{id:guid}", async (PeopleContext db, Guid id, Person person) =>
{
    db.People.Update(person);
    await db.SaveChangesAsync();
});

app.MapDelete("/people/{id:guid}", async (PeopleContext db, Guid id) =>
{
    var person = await db.People.SingleOrDefaultAsync(p => p.Id == id);
    db.People.Remove(person);
    await db.SaveChangesAsync();
});

await app.RunAsync();