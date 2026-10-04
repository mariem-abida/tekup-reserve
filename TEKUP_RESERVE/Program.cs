using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TEKUP_RESERVE.Data;
using TEKUP_RESERVE.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) DbContext avec SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2) Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// 3) MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();
await SeedData.InitializeAsync(app.Services);


// 4) Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 5) Authentification + Autorisation
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

