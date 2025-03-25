using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<P50_4_22.Models.PetStoreRpmContext>(option => 
option.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Profile";
        options.LogoutPath = "/Home/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerPolicy", policy =>
        policy.RequireRole("Customer"));
});

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
