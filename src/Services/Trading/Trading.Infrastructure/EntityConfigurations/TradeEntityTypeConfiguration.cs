using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraLibrium.Services.Trading.Infrastructure.EntityConfigurations;

public class TradeEntityTypeConfiguration
    : IEntityTypeConfiguration<Trade>
{
    public void Configure(EntityTypeBuilder<Trade> builder)
    {
        builder.ToTable("trades", TradingContext.DEFAULT_SCHEMA);

        builder.HasKey(e => e.Id);

        builder.Ignore(e => e.DomainEvents);

        builder.Property(e => e.Id)
            .UseHiLo("entryseq", TradingContext.DEFAULT_SCHEMA);

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
