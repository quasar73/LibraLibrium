using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Trading.Infrastructure.EntityConfigurations;

public class TradeEntryEntityTypeConfiguration
    : IEntityTypeConfiguration<TradeEntry>
{
    public void Configure(EntityTypeBuilder<TradeEntry> builder)
    {
        builder.ToTable("Entries", TradingContext.DEFAULT_SCHEMA);

        builder.HasKey(t => t.Id);

        builder.Ignore(t => t.DomainEvents);

        builder.Property(o => o.Id)
            .UseHiLo("entryseq", TradingContext.DEFAULT_SCHEMA);

        builder
            .Property<int>("TradeId")
            .IsRequired();

        builder
           .Property<int>("_typeId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("TypeId")
           .IsRequired(true);

        builder
           .Property<int>("_bookId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("BookId")
           .IsRequired(true);

        builder
           .Property<string>("_traderId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("TraderId")
           .IsRequired(true);

        builder
            .HasOne<Book>()
            .WithMany()
            .IsRequired(false)
            .HasForeignKey("_bookId");

        builder.HasOne(e => e.Type)
            .WithMany()
            .HasForeignKey("_typeId");
    }
}
