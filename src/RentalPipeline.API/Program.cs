using FluentValidation;
using Microsoft.EntityFrameworkCore;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Application.UseCases.Clients;
using RentalPipeline.Application.UseCases.Properties;
using RentalPipeline.Application.UseCases.Proposals;
using RentalPipeline.Application.UseCases.Auth;
using RentalPipeline.Application.Validators;
using RentalPipeline.Application.Events;
using RentalPipeline.Application.Interfaces;
using RentalPipeline.Application.DTOs.Requests;
using RentalPipeline.Infrastructure.Data;
using RentalPipeline.Infrastructure.Repositories;
using RentalPipeline.Infrastructure.Events;
using RentalPipeline.Infrastructure.Auth;
using RentalPipeline.API.Middlewares;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de dados ──────────────────────────────────────────────────────────
var rawConnectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (rawConnectionString == null)
    throw new InvalidOperationException("Connection string não configurada.");

var connectionString = rawConnectionString;
if (rawConnectionString.StartsWith("postgresql://") || rawConnectionString.StartsWith("postgres://"))
{
    var uri = new Uri(rawConnectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])};SSL Mode=Require;Trust Server Certificate=true";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── Repositórios ────────────────────────────────────────────────────────────
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProposalRepository, ProposalRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ── Use Cases ────────────────────────────────────────────────────────────────
builder.Services.AddScoped<CreatePropertyUseCase>();
builder.Services.AddScoped<CreateClientUseCase>();
builder.Services.AddScoped<CreateProposalUseCase>();
builder.Services.AddScoped<TransitionProposalUseCase>();
builder.Services.AddScoped<GetProposalHistoryUseCase>();
builder.Services.AddScoped<DeleteClientUseCase>();

// ── Autenticação JWT ─────────────────────────────────────────────────────────
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();

// ── Token Service ────────────────────────────────────────────────────────────
builder.Services.AddScoped<ITokenService, TokenService>();

// ── Auth Use Cases ───────────────────────────────────────────────────────────
builder.Services.AddScoped<RegisterUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<MeUseCase>();

// ── Validators ───────────────────────────────────────────────────────────────
builder.Services.AddScoped<IValidator<CreateClientRequest>, CreateClientValidator>();
builder.Services.AddScoped<IValidator<CreatePropertyRequest>, CreatePropertyValidator>();
builder.Services.AddScoped<IValidator<CreateProposalRequest>, CreateProposalValidator>();
builder.Services.AddScoped<IValidator<TransitionProposalRequest>, TransitionProposalValidator>();
builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterValidator>();
builder.Services.AddScoped<IValidator<LoginRequest>, LoginValidator>();

// ── API ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter<CreateClientRequest>>();
    options.Filters.Add<ValidationFilter<CreatePropertyRequest>>();
    options.Filters.Add<ValidationFilter<CreateProposalRequest>>();
    options.Filters.Add<ValidationFilter<TransitionProposalRequest>>();
    options.Filters.Add<ValidationFilter<RegisterRequest>>();
    options.Filters.Add<ValidationFilter<LoginRequest>>();
});

// ── API Docs (Scalar) ────────────────────────────────────────────────────────
builder.Services.AddOpenApi();

// ── Event Publisher ──────────────────────────────────────────────────────────
builder.Services.AddScoped<IEventPublisher, ConsoleEventPublisher>();

// ── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Migrations e Seed ────────────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    await DatabaseSeeder.SeedAsync(db);
}

// ── Middlewares ──────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Rental Pipeline API";
    options.Theme = ScalarTheme.DeepSpace;
    options.WithHttpBearerAuthentication(bearer =>
    {
        bearer.Token = "seu_token_jwt_aqui";
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();