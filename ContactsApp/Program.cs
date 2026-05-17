using Entities;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Reflection;
using ServiceContracts;
using Services;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to IOC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IPersonService, PersonService>();

// Configure Rotativa resources (wkhtmltopdf). UseRotativa does not have a services overload;
// configure Rotativa after building the app.

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseStaticFiles();
app.MapControllers();

// setting up Rotativa for pdf creation - ensure RotativaPath points to the executable
try
{
    var exePath = Path.Combine(app.Environment.WebRootPath, "Rotativa", "wkhtmltopdf.exe");

    // Try to set RotativaPath via public API (Setup) first
    try
    {
        RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");
    }
    catch { /* ignore - we'll set path via reflection below */ }

    var rotativaType = typeof(RotativaConfiguration);

    // Attempt to set common property/field names via reflection
    var prop = rotativaType.GetProperty("RotativaPath", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
    if (prop != null && prop.CanWrite)
    {
        prop.SetValue(null, exePath);
    }
    else
    {
        // backing field for auto-property
        var backing = rotativaType.GetField("<RotativaPath>k__BackingField", BindingFlags.Static | BindingFlags.NonPublic);
        if (backing != null)
        {
            backing.SetValue(null, exePath);
        }
        else
        {
            var privateField = rotativaType.GetField("_rotativaPath", BindingFlags.Static | BindingFlags.NonPublic)
                               ?? rotativaType.GetField("rotativaPath", BindingFlags.Static | BindingFlags.NonPublic)
                               ?? rotativaType.GetField("RotativaPath", BindingFlags.Static | BindingFlags.NonPublic);
            if (privateField != null)
                privateField.SetValue(null, exePath);
        }
    }

    app.UseRotativa();
}
catch (Exception ex)
{
    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine($"Warning: Rotativa/wkhtmltopdf not available: {ex.Message}");
    }
}



app.Run();
