using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Data;

internal class DataEntityConfiguration : IEntityTypeConfiguration<DataEntity>
{
    public void Configure(EntityTypeBuilder<DataEntity> builder)
    {
        builder.ToTable("data");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("id");

        builder.Property(u => u.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.Age)
            .HasColumnName("age")
            .IsRequired();

        builder.Property(u => u.Description)
            .HasColumnName("description")
            .HasMaxLength(500);

        builder.Property(u => u.Title)
            .HasColumnName("title")
            .HasMaxLength(200);

        builder.Property(u => u.Content)
            .HasColumnName("content");
    }
}
