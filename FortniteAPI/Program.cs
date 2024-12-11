using Microsoft.EntityFrameworkCore;
using FortniteAPI.Models;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using FortniteAPI;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Hosting;
using Elfie.Serialization;
using Supabase.Gotrue;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;





var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.TraversePath().Load();
builder.Configuration.AddEnvironmentVariables();



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();  
    logging.AddDebug();    
    logging.AddEventSourceLogger(); 
});

// Add services to the container.

var dbConnection = DotNetEnv.Env.GetString("DB_CONNECTION");
var authConnection = DotNetEnv.Env.GetString("AUTH_CONNECTION");
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");
var authString = Environment.GetEnvironmentVariable("AUTH_CONNECTION");



if (string.IsNullOrEmpty(dbConnection))
{
    throw new InvalidOperationException("Database connection string is not set.");
}

if (string.IsNullOrEmpty(authConnection))
{
    throw new InvalidOperationException("Database connection string is not set.");
}

Console.WriteLine($"DATABASE_URL: {dbConnection}");
Console.WriteLine($"AUTH_URL: {authConnection}");



builder.Services.AddControllers();

builder.Services.AddDbContext<FortniteContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
        npgsqlOptions.CommandTimeout(60)) 
);

builder.Services.AddDbContext<UsersContext>(options =>
    options.UseNpgsql(authString, npgsqlOptions =>
        npgsqlOptions.CommandTimeout(60)) 
);

builder.Logging.AddConsole();



var encodeSecret = DotNetEnv.Env.GetString("ENCODE_SECRET");
var encodeString = Environment.GetEnvironmentVariable("ENCODE_SECRET");




builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ClockSkew = TimeSpan.Zero,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "apiWithAuthBackend",
        ValidAudience = "apiWithAuthBackend",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(encodeString)
            ),
    };
});
builder.Services.AddIdentityCore<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<UsersContext>();
builder.Services.AddScoped<TokenService, TokenService>();



var app = builder.Build();



app.UseDefaultFiles(); 
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();



app.MapControllers();




app.Run();
