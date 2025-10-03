using System;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


var app = builder.Build();
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var orders = new List<Order>();

//app.MapGet("/testing", () => "Hola! just testing");

//AllOrders
app.MapGet("/orders", () => orders);

//OrderById
app.MapGet("/orders/{id}", (Guid id) =>
{
    var uniqueOrder = orders.Find(order => order.Id == id);
    if (uniqueOrder != null)
    {
        return Results.Ok(uniqueOrder);
    }

    return Results.NotFound($"No order found for ID: {id}");
});

//Post order
app.MapPost("/orders", (Order order) =>
{
    order.Id = Guid.NewGuid();
    order.Date = DateTime.UtcNow;

    //Validation
    if (string.IsNullOrEmpty(order.Client))
    {
        return Results.BadRequest("Client is required");
    }
    else if (order.Client.Length <= 2)
    {
        return Results.BadRequest("All characters must be higher than 2");

    }

    if (order.Total <= 0)
    {
        return Results.BadRequest("Total must be higher than 0");
    }
    else if (order.Total is null)
    {
        return Results.BadRequest("Total is required");
    }

    orders.Add(order);
    return Results.Created($"/ order /{order.Id}", order);
});


//update order
app.MapPut("/orders/{id}", (Guid id, Order orderUpdate) =>

{
    if (orderUpdate.Id != id)
    {
        return Results.BadRequest("Invalid ID.");
    }


    var orderFound = orders.Find(order => order.Id == id);

    //validation
    if (string.IsNullOrEmpty(orderUpdate.Client))
    {
        return Results.BadRequest("Client is required");
    }
    else if (orderUpdate.Client.Length <= 2)
    {
        return Results.BadRequest("All characters must be higher than 2");

    }
    if (orderUpdate.Total <= 0)
    {
        return Results.BadRequest("Total must be higher than 0");
    }



    if (orderFound != null)
    {
        orderFound.Client = orderUpdate.Client;
        orderFound.Total = orderUpdate.Total;

        return Results.Ok(orderFound);
    }
    return Results.NotFound($"No order found for ID: {id}");
});

//Delete orders
app.MapDelete("/orders/{id}", (Guid id) =>
{
    var deleteOrder = orders.Find(order => order.Id == id);
    if (deleteOrder != null)
    {
        orders.Remove(deleteOrder);
    }
    else
    {
        return Results.NotFound($"No order found for ID: {id}");
    }

    return Results.NoContent();
});



app.Run();


public class Order
{
    public Guid Id { get; set; }
    public string Client { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal? Total { get; set; }
}

//docs: add README file with usage instructions