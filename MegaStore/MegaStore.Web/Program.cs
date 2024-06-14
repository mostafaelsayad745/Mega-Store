
using MegaStore.DataAccess.Data;
using MegaStore.DataAccess.Implementations;
using MegaStore.Entities.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MegaStore.Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using MegaStore.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

// Add Database Context
builder.Services.AddDbContext<MegaStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add Stripe
builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

builder.Services.AddRazorPages();
builder.Services.AddSingleton<IEmailSender, EmailSender>();
// Add Identity
builder.Services.AddIdentity<IdentityUser,IdentityRole>(options => options.Lockout.DefaultLockoutTimeSpan= TimeSpan.FromHours(4))
    .AddDefaultTokenProviders()
    .AddDefaultUI().AddEntityFrameworkStores<MegaStoreDbContext>();
// Add Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// Add DbInitializer
builder.Services.AddScoped<IDbInitializer, DbInitializer>();

// Add session
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache();

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

StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:SecretKey").Get<string>();
SeedData();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "Customer",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedData()
{
    using(var scope = app.Services.CreateScope())
    {
		
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.Initialize();
	}
}
