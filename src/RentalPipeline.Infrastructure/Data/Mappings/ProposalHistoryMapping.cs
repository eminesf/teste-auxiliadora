using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RentalPipeline.Domain.Entities;

namespace RentalPipeline.Infrastructure.Data.Mappings;

public class ProposalHistoryMapping : IEntityTypeConfiguration<ProposalHistory>
{
    public void Configure(EntityTypeBuilder<ProposalHistory> builder)
    {
        builder.ToTable("proposal_histories");

        builder.HasKey(h => h.Id);

        builder.Property(h => h.Id)
            .HasColumnName("id");

        builder.Property(h => h.ProposalId)
            .HasColumnName("proposal_id")
            .IsRequired();

        builder.Property(h => h.FromStatus)
            .HasColumnName("from_status")
            .IsRequired(false);

        builder.Property(h => h.ToStatus)
            .HasColumnName("to_status")
            .IsRequired();

        builder.Property(h => h.OccurredAt)
            .HasColumnName("occurred_at")
            .IsRequired();

        builder.HasOne<Proposal>()
            .WithMany()
            .HasForeignKey(h => h.ProposalId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}