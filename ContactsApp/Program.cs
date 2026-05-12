using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// Add services to IOC container (use singletons so in-memory data persists across requests)
builder.Services.AddSingleton<ICountriesService,CountriesService>();
builder.Services.AddSingleton<IPersonService,PersonService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}



app.UseStaticFiles();
app.MapControllers();


app.Run();
