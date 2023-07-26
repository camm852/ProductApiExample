using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProductApi.Models;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//SqlServer
builder.Services.AddDbContext<PRODUCTAPIContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));


builder.Services.AddRouting(options => options.LowercaseUrls = true); //url en minusculas

builder.Services.AddControllers().AddJsonOptions(options => //Ignorar referencias ciclicas
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});


//builder.Configuration.AddJsonFile("appsettings.json");
//var secretKey = builder.Configuration.GetSection("JwtSettings").GetSection("SecretKey").ToString();
//var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);

//builder.Services.AddAuthentication(config =>
//{
//    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(config =>
//{
//    config.RequireHttpsMetadata = false;
//    config.SaveToken = true;
//    config.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
//        ValidateIssuer = false,
//        ValidateAudience = false
//    };
//});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
    };
});


var corsRules = "CorsRules";

builder.Services.AddCors(options => //Para los cors
{
    options.AddPolicy(name: corsRules, builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsRules);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
