using Application.Helper;
using Application.Interface;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using Infra;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System;
using System.Globalization;
using System.Reflection;
using Application.Resource;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddLocalization();

builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(options =>
    {
        // استفاده از روش Type-Safe برای پیدا کردن ریسورس در لایه دیگر
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(ValidationMessages));
    });


var useSqlServer = builder.Configuration.GetValue<bool>("UseSqlServer");


var connectionString = useSqlServer 
    ? builder.Configuration.GetConnectionString("SqlServerConnection") 
    : builder.Configuration.GetConnectionString("PostgresConnection");

builder.Services.AddElmah<SqlErrorLog>(options =>
{
    options.ConnectionString =
        builder.Configuration.GetConnectionString("SqlServerConnection");
    options.Path = "elmah";
});

builder.Services.AddMemoryCache();//
builder.Services.AddOutputCache();///
builder.Services.AddResponseCaching();

builder.Services.AddDataProtection();

//builder.Services.AddLocalization(options =>
//{
//    options.ResourcesPath = "Resources";
//});///////



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

app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");



app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseElmah(); // فعال‌سازی ELMAH

var supportedCultures = new[]
{
    new CultureInfo("fa"),
    new CultureInfo("en")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fa"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseRouting();

app.UseOutputCache();//
app.UseResponseCaching();//

app.UseAuthorization();




app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
