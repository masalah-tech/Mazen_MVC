using Mazen.DataAccess.Repository;
using Mazen.DataAccess.Repository.IRepository;
using MazenWebApp.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using MazenWebApp.Utility;
using Stripe;
using Microsoft.EntityFrameworkCore.Internal;
using MazenWebApp.DataAccess.DBInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(
    o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

builder.Services.ConfigureApplicationCookie(
    o =>
    {
        o.LoginPath = @"/Identity/Account/Login";
        o.LogoutPath = @"/Identity/Account/Logout";
        o.AccessDeniedPath = @"/Identity/Account/AccessDenied";
    }
);

builder.Services.AddAuthentication().AddFacebook(o =>
{
    o.AppId = "354815460631779";
    o.AppSecret = "06dd1d455c4d1bd26c6e00fb4d450b93";
});

builder.Services.AddAuthentication().AddMicrosoftAccount(o =>
{
    o.ClientId = "2d1dab95-dd6c-4b9b-8644-edae69633371";
    o.ClientSecret = "yzp8Q~c~TtNiLxKb_OYtSBxMtmqZ-5uVxUznebI1";
});


builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.IdleTimeout = TimeSpan.FromMinutes(100);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true; 
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddRazorPages();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

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

StripeConfiguration.ApiKey = 
    builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDatabase();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void SeedDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();

        dbInitializer.Initialize();
    }
}
