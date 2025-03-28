using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using P50_4_22.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ��������� ��������� ���� ������
builder.Services.AddDbContext<PetStoreRpmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));

// ��������� ���
builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

// ��������� ������ (��������� �� Build())
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1); // ����� ����� ������
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ��������� ��������������
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Home/Profile";
        options.LogoutPath = "/Home/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

// ��������� �����������
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerPolicy", policy =>
        policy.RequireRole("Customer"));
});

// ������ ����������
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ���������� ������
app.UseSession();

app.UseRouting();

// ���������� �������������� � �����������
app.UseAuthentication();
app.UseAuthorization();

// ��������� ���������
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();