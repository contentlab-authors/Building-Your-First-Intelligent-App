using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using UserFeedbackApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Add Database Context
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlServer(builder.Configuration["userfeedbackdatabase"]));


    builder.Services.AddAzureClients(clientBuilder =>
        clientBuilder.AddTextAnalyticsClient(new Uri(builder.Configuration["Cognitive_EndPoint"]),
        new Azure.AzureKeyCredential(builder.Configuration["Cognitive_Key"]))
        );
}
else
{
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlServer(Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING")));

    builder.Services.AddAzureClients(clientBuilder =>
        clientBuilder.AddTextAnalyticsClient(new Uri(builder.Configuration["Cognitive_EndPoint"]),
        new Azure.AzureKeyCredential(builder.Configuration["Cognitive_Key"]))
        );
}

// Add services to the container.
builder.Services.AddControllersWithViews();



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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
