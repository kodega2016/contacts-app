using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Add services to IOC container
builder.Services.AddScoped<ICountriesService,CountriesService>();
builder.Services.AddScoped<IPersonService,PersonService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}


app.UseStaticFiles();
app.MapControllers();


app.Run();
