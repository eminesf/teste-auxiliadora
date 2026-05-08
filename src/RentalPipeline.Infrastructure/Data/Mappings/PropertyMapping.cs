using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Infrastructure.Data.Mappings;

public class PropertyMapping : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("properties");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.Street).HasColumnName("street").IsRequired().HasMaxLength(300);
        builder.Property(p => p.Number).HasColumnName("number").IsRequired().HasMaxLength(20);
        builder.Property(p => p.Complement).HasColumnName("complement").HasMaxLength(200).IsRequired(false);
        builder.Property(p => p.District).HasColumnName("district").IsRequired().HasMaxLength(200);
        builder.Property(p => p.City).HasColumnName("city").IsRequired().HasMaxLength(200);
        builder.Property(p => p.State).HasColumnName("state").IsRequired().HasMaxLength(2);
        builder.Property(p => p.RentPrice).HasColumnName("rent_price").HasColumnType("numeric(10,2)").IsRequired();
        builder.Property(p => p.Status).HasColumnName("status").IsRequired();
        builder.Property(p => p.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(p => p.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.HasOne(p => p.Owner)
            .WithMany()
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}