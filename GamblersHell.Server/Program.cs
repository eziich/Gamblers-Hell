using GamblersHell.Server.Services;
using GamblersHell.Services;
using GamblersHell.Shared.Interface;
using MudBlazor.Services;
using Microsoft.AspNetCore.Authentication.Cookies; // For cookie-based authentication
using GamblersHell.Shared;
using System.Reflection;
using GamblersHell.Server.Interface;
using GamblersHell.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Ensure user-secrets are loaded in development before any configuration is accessed
builder.Host.ConfigureAppConfiguration((hostingContext, config) =>
{
    if (hostingContext.HostingEnvironment.IsDevelopment())
    {
        config.AddUserSecrets<Program>();
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddMudServices();


// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Ensure this line is present to configure Swagger.


// Read the connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Initialize database
DatabaseContext.InitializeDatabase(connectionString);

// Register UserService with the connection string
builder.Services.AddScoped<UserService>(provider => 
    new UserService(
        connectionString,
        provider.GetRequiredService<IEmailSenderInterface>()
    ));

builder.Services.AddHostedService<TimedHostedService>();


// Add authentication and cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = GamblersHellConstants.GamblersHellCookieName;
        options.LoginPath = "/Auth/Login"; // Define your login URL here
        options.LogoutPath = "/Auth/Logout"; // Define your logout URL here
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set the session expiration
        options.SlidingExpiration = true; // Optionally enable sliding expiration
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

// Add authorization services
builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

// Register services
builder.Services.AddScoped<IAuthRepository, AuthService>();
builder.Services.AddScoped<IEmailSenderInterface, EmailSenderService>();
builder.Services.AddScoped<BlackjackService>();
builder.Services.AddScoped<SlotsService>();
builder.Services.AddScoped<PokerService>();
builder.Services.AddScoped<CardWarsService>();
builder.Services.AddScoped<RiddleService>();
builder.Services.AddScoped<LadyService>();
builder.Services.AddScoped<PandoraService>();
builder.Services.AddScoped<HereticsRouletteService>();




// With this line
builder.Services.AddScoped<HellPigService>(provider =>
    new HellPigService(
        provider.GetRequiredService<IWebHostEnvironment>(),
        connectionString  // Use the connectionString variable you already have
    )
);

builder.Services.AddScoped<TransactionService>(provider =>
    new TransactionService(
        provider.GetRequiredService<IWebHostEnvironment>(),
        builder.Configuration.GetConnectionString("DefaultConnection"),
        provider.GetRequiredService<UserService>()
    ));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // You may need to restrict this in production
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // Ensure Swagger middleware is added for the API docs
    app.UseSwaggerUI();  // Ensure SwaggerUI middleware is added for the Swagger UI
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();  // Serve static files from wwwroot

// Use authentication and authorization middlewares
app.UseAuthentication();  // Ensure this is called before UseAuthorization
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.UseCors("AllowAll");

app.Run();
