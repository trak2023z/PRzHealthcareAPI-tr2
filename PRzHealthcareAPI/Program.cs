using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using PRzHealthcareAPI;
using PRzHealthcareAPI.Middlewares;
using PRzHealthcareAPI.Models;
using PRzHealthcareAPI.Services;
using System.Text;
using System.Text.Json.Serialization;
using System.Web.Services.Description;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://192.168.56.1:5000");


var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();


var authenticationSettings = new AuthenticationSettings();
configuration.GetSection("Authentication").Bind(authenticationSettings);

var emailSettings = new EmailSettings();
configuration.GetSection("MailSettings").Bind(emailSettings);

builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddSingleton(emailSettings);
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
});
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddControllers().AddFluentValidation().AddJsonOptions(x => { x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

/*  Services    */
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IVaccinationService, VaccinationService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICertificateService, CertificateService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICaptchaService, CaptchaService>();
builder.Services.AddHttpClient<CaptchaService>();
builder.Services.AddScoped<HealthcareSeeder>();

builder.Services.AddScoped<IPasswordHasher<Account>, PasswordHasher<Account>>();
builder.Services.AddDbContext<HealthcareDbContext>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddCors(op =>
{
    op.AddPolicy("FrontEndClient", builder =>
        builder.AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin());
});
builder.Services.AddMvc();

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<HealthcareSeeder>();
seeder.Seed();

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHFqVk9rXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRcQlliTH5Xd0FhUXxbd3M=;Mgo+DSMBPh8sVXJ1S0d+X1hPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXpSc0VnWHtceHdTT2c=;ORg4AjUWIQA/Gnt2VFhhQlJNfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Xd0BjXX5acnxWTmFb;MTczMjMxM0AzMjMxMmUzMTJlMzMzOW5UbzRQZVlzUXdrYkFGSFI5bXJxdDZleWRmYVMwSW8rTExNV1h3aEI2Qnc9;MTczMjMxNEAzMjMxMmUzMTJlMzMzOWpYNmg2SkxrQUZueGFRV1NlMmI2S2lHOFVucGJQLzFsUTFDSGZUMEkyM0U9;NRAiBiAaIQQuGjN/V0d+XU9Hf1RDX3xKf0x/TGpQb19xflBPallYVBYiSV9jS31TckRmWXtfdHZdQmReVg==;MTczMjMxNkAzMjMxMmUzMTJlMzMzOW5COUw1eWlkRnZHQ3ZodkMzUTJtRGN5Yk9zU2o2bzcrOG83MzY0dFRyUzQ9;MTczMjMxN0AzMjMxMmUzMTJlMzMzOVN0VHZqUFQwZU5MaklKMjczc0hDaWwxTi9aWTA4d2srRGVOMW16NUEyejQ9;Mgo+DSMBMAY9C3t2VFhhQlJNfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Xd0BjXX5acnxQR2Fb;MTczMjMxOUAzMjMxMmUzMTJlMzMzOUpyQVdtMURqT3hXMk5ORmNYM3IvbXhBY2M3UnFwanh1dkdIN2dsYjBKd0k9;MTczMjMyMEAzMjMxMmUzMTJlMzMzOW84VG0xQ0ZIZFZIOE93R3BxN1JmK0RzVzNmMmZNTWF4b01lclREcDVVZEE9;MTczMjMyMUAzMjMxMmUzMTJlMzMzOW5COUw1eWlkRnZHQ3ZodkMzUTJtRGN5Yk9zU2o2bzcrOG83MzY0dFRyUzQ9");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "PRzHealthcareAPI");
    });
}

app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseRouting();
app.UseCors("FrontEndClient");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
