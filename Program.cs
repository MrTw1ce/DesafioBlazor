using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using App1.Components;

using Microsoft.EntityFrameworkCore;
using SQLModel.NETCoreWebAPI.ApplicationDbContext;
using SQLModel.App1.Models;

var builder = WebApplication.CreateBuilder(args);

var cartContent = new List<ProductOrder>{
    //new ProductOrder(1,"Eggs",1,5),
    //new ProductOrder(2,"Cake",10,1),
    //new ProductOrder(3,"Tart",5,2)
};

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddControllers();
//builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()){
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseAntiforgery();
app.UseHttpsRedirection();

app.MapGet("/products", (AppDbContext dbContext) => {
    var result = dbContext.Products.ToList();
    var products = new List<Products>();
    for(int i = 0;i<result.Count;i++){
        products.Add(new Products(JsonSerializer.Deserialize<Images>(result[i].Image),result[i].Name,result[i].Category,result[i].Price));
    }
    return Results.Ok(products);
});

app.MapGet("/cart", () =>{
    return cartContent;
});

app.MapPost("/cart", (ProductOrder order) => {
    if(cartContent.Count() == 0){
        cartContent.Add(new ProductOrder(1, order.Product, order.Price, 1));
    }
    else{
        cartContent.Add(new ProductOrder(cartContent.Max(r => r.Id) + 1, order.Product, order.Price, 1));
    }
    return Results.Ok(cartContent);
});

app.MapPut("/cart/{id:int}", (int id, ProductOrder order) => {
    var targetOrder = cartContent.SingleOrDefault(t => id == t.Id);
    if (targetOrder is null){
        return Results.BadRequest("The product isn't in the cart!");
    }
    if ((order.Quantity < 1) || (order.Quantity > 99)){
        return Results.BadRequest("Invalid quantity!");
    }
    var orderIndex = cartContent.IndexOf(targetOrder);
    cartContent[orderIndex] = new ProductOrder(targetOrder.Id, targetOrder.Product, order.Price, order.Quantity);
    return Results.Ok(cartContent);
});

app.MapDelete("/cart/{id:int}", (int id) => {
    var targetOrder = cartContent.SingleOrDefault(t => id == t.Id);
    if (targetOrder is null){
        return Results.BadRequest("The product isn't in the cart!");
    }
    cartContent.Remove(targetOrder);
    return Results.Ok(cartContent);
});

app.MapDelete("/cart", () => {
    cartContent.Clear();
    return Results.Ok(cartContent);
});

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();


public record Products(Images Image, string Name, string Category, float Price);
public record Images(string thumbnail,string mobile, string tablet, string desktop);
public record ProductOrder(int Id, string Product, float Price, int Quantity);
[JsonSerializable(typeof(List<ProductOrder>))]
internal partial class ProductOrderContext : JsonSerializerContext{}