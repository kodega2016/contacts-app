using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ICountriesService,CountriesService>();
builder.Services.AddScoped<IPersonService,PersonService>();


var app = builder.Build();




app.UseStaticFiles();
app.MapControllers();


app.Run();
