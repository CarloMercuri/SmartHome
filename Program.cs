using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartHome.Security.KeyStorage;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

ISecretsStorage _secrets = new LocalSecretsStorage();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _secrets.GetIssuer(), 
        ValidAudience = _secrets.GetAudience(), 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secrets.GetMainSecurityKey())), 
        ClockSkew = TimeSpan.Zero 
    };
});



var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//app.MapGet("/protected", () => "This is a protected resource!")
//   .RequireAuthorization();

app.MapControllers();

app.Run();
