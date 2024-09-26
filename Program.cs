using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyCompany.Domain;
using MyCompany.Domain.Repositories.Abstract;
using MyCompany.Domain.Repositories.EntityFramework;
using MyCompany.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("AdminArea", policy => { policy.RequireRole("admin");});
});
// Add services to the container.
builder.Services.AddControllersWithViews(x =>
{
    x.Conventions.Add(new AdminAreaAuthorization("Admin", "AdminArea"));
});


builder.Services.AddTransient<DataManager>();
builder.Services.AddTransient<IServiceItemsRepository, EFServiceItemsRepository>();
builder.Services.AddTransient<ITextFieldsRepository, EFTextFieldsRepository>();

var config = new Config();
builder.Configuration.Bind("Project", config);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(opts =>
{
    opts.User.RequireUniqueEmail = true;
    opts.Password.RequiredLength = 6;
    opts.Password.RequireNonAlphanumeric = false;
    opts.Password.RequireLowercase = false;
    opts.Password.RequireUppercase = false;
    opts.Password.RequireDigit = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "myCompanyAuth";
    options.Cookie.HttpOnly = true;
    options.LoginPath = "/account/login";
    options.AccessDeniedPath = "/account/accessdenied";
    options.SlidingExpiration = true;
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

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
