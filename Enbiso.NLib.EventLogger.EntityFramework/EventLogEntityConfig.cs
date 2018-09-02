using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enbiso.NLib.EventLogger.EntityFramework
{
    public class EventLogEntityConfig : IEntityTypeConfiguration<EventLog>
    {
        public void Configure(EntityTypeBuilder<EventLog> builder)
        {
            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.Content)
                .IsRequired();

            builder.Property(e => e.CreationTime)
                .IsRequired();

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.TimesSent)
                .IsRequired();

            builder.Property(e => e.EventTypeName)
                .IsRequired();
        }
    }

}
