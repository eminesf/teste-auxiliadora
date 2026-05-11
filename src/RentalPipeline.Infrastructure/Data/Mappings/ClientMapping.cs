using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Infrastructure.Data.Mappings;

public class ClientMapping : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id");

        builder.Property(c => c.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Document)
            .HasColumnName("document")
            .IsRequired()
            .HasMaxLength(14);

        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.HasIndex(c => c.Document)
            .IsUnique()
            .HasDatabaseName("IX_clients_document_unique");

        builder.Property(c => c.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        builder.Property(c => c.Role)
            .HasColumnName("role")
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.Email)
            .IsUnique()
            .HasDatabaseName("IX_clients_email_unique");
    }
}