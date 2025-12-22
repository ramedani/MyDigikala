using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using Infra;
using Application.Interface;
using Application.Helper;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
var useSqlServer = builder.Configuration.GetValue<bool>("UseSqlServer");


var connectionString = useSqlServer 
    ? builder.Configuration.GetConnectionString("SqlServerConnection") 
    : builder.Configuration.GetConnectionString("PostgresConnection"); 


builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (useSqlServer)
    {
       
        options.UseSqlServer(connectionString);
    }
    else
    {
        options.UseNpgsql(connectionString);
    }
});


builder.Services.AddScoped<ICategory , CategoryService>();
builder.Services.AddScoped<ISiteSetting, SiteSettingService>();
builder.Services.AddScoped<IProducts, ProductService>();
builder.Services.AddScoped<INews, NewsService>();
builder.Services.AddScoped<INewsCategory, NewsCategoryService>();
builder.Services.AddScoped<IFileSecurityHelper, FileSecurityHelper>(); 
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
