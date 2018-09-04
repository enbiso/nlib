using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Enbiso.NLib.Idempotency.EntityFramework
{
    public class ClientRequestEntityConfig : IEntityTypeConfiguration<RequestLog>
    {
        public void Configure(EntityTypeBuilder<RequestLog> requestConfiguration)
        {
            requestConfiguration.Property(cr => cr.Name).IsRequired();
            requestConfiguration.Property(cr => cr.Time).IsRequired();
        }
    }

}
