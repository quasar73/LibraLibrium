using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Trading.Infrastructure.EntityConfigurations;

public class TradeEntryEntityTypeConfiguration
    : IEntityTypeConfiguration<TradeEntry>
{
    public void Configure(EntityTypeBuilder<TradeEntry> builder)
    {
        builder.ToTable("orders", TradingContext.DEFAULT_SCHEMA);

        builder.HasKey(o => o.Id);

        builder.Ignore(b => b.DomainEvents);

        builder.Property(o => o.Id)
            .UseHiLo("tradeseq", TradingContext.DEFAULT_SCHEMA);

        builder
            .Property<int>("TradeId")
            .IsRequired();

        builder
           .Property<string>("_typeId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("TypeId")
           .IsRequired(true);

        builder
           .Property<string>("_bookId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("BookId")
           .IsRequired(true);

        builder
           .Property<string>("_traderId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("TraderId")
           .IsRequired(true);
    }
}
