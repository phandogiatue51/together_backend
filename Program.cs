using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Together.Helpers;
using Together.Models;
using Together.Repositories;
using Together.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient("MLClient", client =>
{
    client.BaseAddress = new Uri("http://localhost:8000/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
}); builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TogetherDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (builder.Environment.IsDevelopment())
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        var host = Environment.GetEnvironmentVariable("DB_HOST") ??
                   "ep-muddy-unit-a11teyim-pooler.ap-southeast-1.aws.neon.tech";
        var db = Environment.GetEnvironmentVariable("DB_NAME") ?? "neondb";
        var user = Environment.GetEnvironmentVariable("DB_USER") ?? "neondb_owner";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");

        var prodConnectionString = $"Host={host};Database={db};Username={user};Password={password};SslMode=Require;Trust Server Certificate=true";
        options.UseNpgsql(prodConnectionString);
    }

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

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
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "https://localhost:3000",
            "http://localhost:5173",
            "https://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddSingleton<CloudinaryService>();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<AccountRepo>();
builder.Services.AddScoped<BlogRepo>();
builder.Services.AddScoped<OrganizationRepo>();
builder.Services.AddScoped<ProjectRepo>();
builder.Services.AddScoped<StaffRepo>();
builder.Services.AddScoped<CategoryRepo>();
builder.Services.AddScoped<CertificateRepo>();
builder.Services.AddScoped<ApplicationRepo>();

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<StaffService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CertificateService>();
builder.Services.AddScoped<ApplicationService>();

builder.Services.AddScoped<PasswordHelper>();
builder.Services.AddScoped<CalculateScore>();
builder.Services.AddScoped<HourRepo>();

builder.Services.AddMemoryCache();
builder.Services.AddScoped<QrService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReactApp");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
