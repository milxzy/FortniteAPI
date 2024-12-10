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





var builder = WebApplication.CreateBuilder(args);
DotNetEnv.Env.TraversePath().Load();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin() // Allow specific origin
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();  // Log to the console
    logging.AddDebug();    // Log to the debug output window
    logging.AddEventSourceLogger(); // Log to EventSource for diagnostics
});

// Add services to the container.

var dbConnection = DotNetEnv.Env.GetString("DB_CONNECTION");
var authConnection = DotNetEnv.Env.GetString("AUTH_CONNECTION");

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
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddDbContext<FortniteContext>(opt =>
    opt.UseNpgsql(dbConnection));
builder.Services.AddDbContext<UsersContext>(options =>
          options.UseNpgsql(authConnection));
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(option =>
//{
//    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Fortnite API", Version = "v1" });
//    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter a valid token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id="Bearer"
//                }
//            },
//            new string[]{}
//        }
//    });
//});
builder.Logging.AddConsole();



var encodeSecret = DotNetEnv.Env.GetString("ENCODE_SECRET");

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
            Encoding.UTF8.GetBytes(encodeSecret)
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

//Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FortniteAPI v1");
//    });
//}


//app.UseSwagger();
//app.UseSwaggerUI();


app.UseDefaultFiles(); // Looks for index.html, default.html, etc.
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors();

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "public")),
//    RequestPath = "/index.html"  // Set an optional request path if you want a specific URL prefix (e.g., "/static")
//});

app.MapControllers();




app.Run();
