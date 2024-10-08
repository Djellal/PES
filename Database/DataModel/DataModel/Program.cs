using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Configuration;
using DataModel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<DataModelContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DataModelContext") ?? throw new InvalidOperationException("Connection string 'DataModelContext' not found.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


app.Run();
