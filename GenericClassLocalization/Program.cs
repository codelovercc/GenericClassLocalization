using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GenericClassLocalization.Data;
using GenericClassLocalization.Localization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources")
    .AddGenericTypeSupportStringLocalizerFactory();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseRequestLocalization(options =>
{
    options.SupportedCultures = new List<CultureInfo>
    {
        CultureInfo.GetCultureInfo("en-US"),
        CultureInfo.GetCultureInfo("fr-FR"),
        CultureInfo.GetCultureInfo("es-ES")
    };
    options.SupportedUICultures = options.SupportedCultures;
    options.DefaultRequestCulture = new RequestCulture(CultureInfo.GetCultureInfo("en-US"));
    options.ApplyCurrentCultureToResponseHeaders = true;
    options.AddInitialRequestCultureProvider(new IetfMappedRequestCultureProvider { Options = options });
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Localization}/{action=HelloWorld}");
app.MapRazorPages();

app.Run();