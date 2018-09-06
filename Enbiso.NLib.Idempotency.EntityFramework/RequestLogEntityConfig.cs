using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enbiso.NLib.Idempotency.EntityFramework
{
    public class RequestLogEntityConfig : IEntityTypeConfiguration<RequestLog>
    {
        public void Configure(EntityTypeBuilder<RequestLog> requestConfiguration)
        {
            requestConfiguration.Property(cr => cr.Name).IsRequired();
            requestConfiguration.Property(cr => cr.Time).IsRequired();
        }
    }

    /// <summary>
    /// Model builder extensions
    /// </summary>
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ApplyIdempotencyConfiguration(this ModelBuilder builder)
        {
            return builder.ApplyConfiguration(new RequestLogEntityConfig());
        }
    }
}
