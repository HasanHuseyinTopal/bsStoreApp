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
using WebAP�.ServiceRegistiration;

var builder = WebApplication.CreateBuilder(args);

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

builder.Services.AddControllers(config =>
{
    config.ReturnHttpNotAcceptable = true; // �stemcinin g�nderilen veri tipini kabul etmedi�i durumlarda 406 not acceptable hatas�n� f�rlat�r
    config.RespectBrowserAcceptHeader = true; // Sunucu talep edilen i�erik tipini otomatik olarak d�n�d�r�r (yada en yak�n olan�)
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
    opt.SuppressModelStateInvalidFilter = true; // Validasyon hatalar�nda default hata yap�s�n� ezmek i�in kullan�l�r
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
