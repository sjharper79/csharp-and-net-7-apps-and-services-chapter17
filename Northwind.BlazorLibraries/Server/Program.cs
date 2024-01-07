using Microsoft.AspNetCore.Mvc;
using Packt.Shared;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<HttpJsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddNorthwindContext();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapGet("api/categories", (
    [FromServices] NorthwindContext db) =>
        Results.Json(db.Categories.Include(c => c.Products)))
    .WithName("GetCategories")
    .Produces<Category[]>(StatusCodes.Status200OK);

app.MapGet("api/orders", (
    [FromServices] NorthwindContext db) =>
        Results.Json(db.Orders.Include(o => o.OrderDetails)))
    .WithName("GetOrders")
    .Produces<Order[]>(StatusCodes.Status200OK);

app.MapGet("api/employees/",
    ([FromServices] NorthwindContext db) =>
        Results.Json(db.Employees))
    .WithName("GetEmployees")
    .Produces<Employee[]>(StatusCodes.Status200OK);

app.MapGet("api/countries/",
    ([FromServices] NorthwindContext db) =>
        Results.Json(db.Employees.Select(emp => emp.Country).Distinct()))
    .WithName("GetCountries")
    .Produces<string[]>(StatusCodes.Status200OK);

app.MapGet("api/cities/", (
    [FromServices] NorthwindContext db) =>
        Results.Json(db.Employees.Select(emp => emp.City).Distinct()))
    .WithName("GetCities")
    .Produces<string[]>(StatusCodes.Status200OK);

app.MapPut("api/employees/{id:int}", async (
    [FromRoute] int id,
    [FromBody] Employee employee,
    [FromServices] NorthwindContext db) =>
    {
        Employee? foundEmployee = await db.Employees.FindAsync(id);
        if (foundEmployee is null) return Results.NotFound();

        foundEmployee.Address = employee.Address;
        foundEmployee.BirthDate = employee.BirthDate;
        foundEmployee.City = employee.City;
        foundEmployee.Country = employee.Country;
        foundEmployee.FirstName = employee.FirstName;
        foundEmployee.HireDate = employee.HireDate;
        foundEmployee.LastName = employee.LastName;
        foundEmployee.Notes = employee.Notes;
        foundEmployee.PostalCode = employee.PostalCode;
        foundEmployee.Region = employee.Region;
        foundEmployee.ReportsTo = employee.ReportsTo;
        foundEmployee.Title = employee.Title;
        foundEmployee.TitleOfCourtesy = employee.TitleOfCourtesy;

        int affected = await db.SaveChangesAsync();

        return Results.Json(affected);
    })
    .Produces(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
