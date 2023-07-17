using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Trading.Infrastructure.EntityConfigurations;

public class TradeEntityTypeConfiguration
    : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.ToTable("orders", TradingContext.DEFAULT_SCHEMA);

        builder.HasKey(o => o.Id);

        builder.Ignore(b => b.DomainEvents);

        builder.Property(o => o.Id)
            .UseHiLo("tradeseq", TradingContext.DEFAULT_SCHEMA);

        builder
           .Property<string>("_senderId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("SenderId")
           .IsRequired(true);

        builder
           .Property<string>("_receiverId")
           .UsePropertyAccessMode(PropertyAccessMode.Field)
           .HasColumnName("ReceiverId")
           .IsRequired(true);

        var navigation = builder.Metadata.FindNavigation(nameof(Trade.Entries));
        navigation!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
