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

// ── Banco de dados (Lógica corrigida para Railway) ──────────────────────────

// 1. Tenta pegar da variável padrão do Railway (DATABASE_URL) 
// 2. Se não existir, tenta o padrão do .NET (ConnectionStrings__DefaultConnection)
// 3. Se não existir, tenta o appsettings.json
var rawConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                         ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                         ?? builder.Configuration.GetConnectionString("DefaultConnection");

string finalConnectionString = rawConnectionString;

// Converte formato URL (postgresql://...) para formato Npgsql se necessário
if (!string.IsNullOrEmpty(rawConnectionString) && rawConnectionString.StartsWith("postgresql://"))
{
    try
    {
        var uri = new Uri(rawConnectionString);
        var userInfo = uri.UserInfo.Split(':');
        var user = userInfo[0];
        var password = Uri.UnescapeDataString(userInfo[1]);
        var host = uri.Host;
        var port = uri.Port;
        var database = uri.AbsolutePath.TrimStart('/');

        finalConnectionString = $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao parsear DATABASE_URL: {ex.Message}");
    }
}

if (string.IsNullOrEmpty(finalConnectionString))
{
    // Isso vai travar o deploy e mostrar o erro real nos logs do Railway
    throw new Exception("CRITICAL: Connection String não encontrada! Verifique as variáveis no Railway.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(finalConnectionString));

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
builder.Services.AddScoped<IEventPublisher, ConsoleEventPublisher>();

// ── CORS ─────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

// ── Migrations automáticas (Essencial para o Railway) ────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<AppDbContext>();
        Console.WriteLine("Aplicando migrations...");
        db.Database.Migrate();
        Console.WriteLine("Migrations aplicadas com sucesso.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao aplicar migrations: {ex.Message}");
        // Em produção, às vezes é melhor o app subir mesmo com erro de migração 
        // para você conseguir debugar, mas aqui ele vai logar o erro.
    }
}

// ── Middlewares ──────────────────────────────────────────────────────────────
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors();

// Habilitar Swagger em produção no Railway para facilitar seus testes iniciais
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    c.RoutePrefix = string.Empty; // Faz o swagger abrir na raiz do domínio
});

app.MapControllers();

// Garante que o app ouça na porta que o Railway designar
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Run($"http://0.0.0.0:{port}");