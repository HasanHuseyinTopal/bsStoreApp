using EntityLayer.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using Repositories;
using Services.Abstract;
using System.Text;
using WebAPÝ.ServiceRegistiration;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers(config =>
{
    config.ReturnHttpNotAcceptable = true; // Ýstemcinin gönderilen veri tipini kabul etmediði durumlarda 406 not acceptable hatasýný fýrlatýr
    config.RespectBrowserAcceptHeader = true; // Sunucu talep edilen içerik tipini otomatik olarak dönüdürür (yada en yakýn olaný)
    config.CacheProfiles.Add("5mins", new CacheProfile() { Duration = 300 });
}).AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly).AddXmlDataContractSerializerFormatters().AddCustomCsvFormatter();

builder.Services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));
builder.Services.AddIdentity<User, IdentityRole>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequiredLength = 5;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireDigit = false;
}).AddEntityFrameworkStores<RepositoryContext>().AddDefaultTokenProviders();
builder.Services.ExtensionService();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidIssuer = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF32.GetBytes(builder.Configuration["JWT:SecretKey"])),
    };
});
builder.Services.AddAuthorization();

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.SuppressModelStateInvalidFilter = true; // Validasyon hatalarýnda default hata yapýsýný ezmek için kullanýlýr
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerService>();
app.UseCustomExceptionHandler(logger);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("CorsPolicy");

app.UseResponseCaching();
//app.UseHttpCacheHeaders();

app.UseRateLimiter();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
