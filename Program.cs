using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using ContosoUniversity.WebApplication.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Add DbContext (MUST be before Build)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetSection("ConnectionStrings")["DefaultConnection"]));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

if (builder.Configuration["URLAPI"] != null)
{
    builder.Services.AddHttpClient("client", client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["URLAPI"]);
    });
}
else
{
    var section = builder.Configuration.GetSection("Api");
    builder.Services.AddHttpClient("client", client =>
    {
        client.BaseAddress = new Uri(section["Address"]);
    });
}

builder.Services.AddRazorPages();

if (builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"] != null)
{
    builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
}
else
{
    builder.Services.AddApplicationInsightsTelemetry();
}

var app = builder.Build();

app.UseExceptionHandler("/Error");
app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();