using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schimb_Valutar_API.DAL.Initialization;
using Schimb_Valutar_API.Services.Interfaces;
using Schimb_Valutar_API.Services;
using Schimb_Valutar_API.DAL.Repositories.Interfaces;
using Schimb_Valutar_API.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHangfire(configuration =>
    configuration.UseSqlServerStorage(builder.Configuration.GetConnectionString("Develop"))
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings());
builder.Services.AddHangfireServer();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000");
                      });
});

builder.Services.AddHttpClient();
builder.Services.AddTransient<ICurrencyRepository, CurrencyRepository>();
builder.Services.AddTransient<ICurrencyService, CurrencyService>();
builder.Services.AddTransient<ICurrencyRateService, CurrencyRateService>();

Startup.Init(builder.Configuration, builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<ICurrencyService>(
    x => x.UpdateAllCurrenciesFromBnrAsync(),
    "0 11 * * *");

app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();
app.Run();
