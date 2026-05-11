using Microsoft.EntityFrameworkCore;
using RentalPipeline.Domain.Entities;
using RentalPipeline.Domain.Enums;

namespace RentalPipeline.Infrastructure.Data;

public static class DatabaseSeeder
{
  public static async Task SeedAsync(AppDbContext context)
  {
    // Só cria se não existir
    var adminExists = await context.Clients
        .AnyAsync(c => c.Document == "00000000000");

    if (adminExists) return;

    var admin = new Client(
        name: "admin",
        email: "admin@admin.com",
        document: "00000000000",
        password: "admin",
        role: ClientRole.AdminMaster
    );

    context.Clients.Add(admin);
    await context.SaveChangesAsync();
  }
}