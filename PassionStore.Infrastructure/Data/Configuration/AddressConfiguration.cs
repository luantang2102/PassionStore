using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data.Configuration.Base;

namespace PassionStore.Infrastructure.Data.Configuration
{
    public class AddressConfiguration : BaseEntityConfiguration<Address>

    {
        public override void Configure(EntityTypeBuilder<Address> builder)
        {
            base.Configure(builder);
        }
    }
}