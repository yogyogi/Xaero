using Microsoft.EntityFrameworkCore;
using Xaero.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MovieContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "PagingPageOne",
    pattern: "{controller}",
    defaults: new { action = "Index", id = 1 })
    .WithStaticAssets();

app.MapControllerRoute(
    name: "Paging",
    pattern: "{controller}/{id:int?}",
    defaults: new { action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
