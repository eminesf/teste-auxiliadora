using Microsoft.EntityFrameworkCore;
using RentalPipeline.Application.Interfaces.Repositories;
using RentalPipeline.Application.UseCases.Clients;
using RentalPipeline.Application.UseCases.Properties;
using RentalPipeline.Application.UseCases.Proposals;
using RentalPipeline.Application.Events;
using RentalPipeline.Infrastructure.Data;
using RentalPipeline.Infrastructure.Repositories;
using RentalPipeline.Infrastructure.Events;
using RentalPipeline.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de dados ──────────────────────────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Converte formato URL (Railway) para formato Npgsql se necessário
if (connectionString != null && connectionString.StartsWith("postgresql://"))
{
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
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

// ── API ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Event Publisher
builder.Services.AddScoped<IEventPublisher, ConsoleEventPublisher>();

// ── CORS (para o frontend vibecodado) ───────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Migrations automáticas na inicialização ──────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── Middlewares ──────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();