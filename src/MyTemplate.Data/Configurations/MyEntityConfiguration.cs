using Aertssen.Framework.Data.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTemplate.Domain.Model;
using MyTemplate.Domain.Validations;

namespace MyTemplate.Data.Configurations
{
    internal class MyEntityConfiguration : AuditableIdentifiableMapping<MyEntity>
    {
        public override void Configure(EntityTypeBuilder<MyEntity> builder)
        {
            base.Configure(builder);

            builder.ToTable("MyEntities", DemoDbConstants.SchemaName.Default);
            
            builder.Property(e => e.Name)
                .HasMaxLength(ValidationConstants.MyEntityNameMaximumLength)
                .IsRequired();
        }
    }
}
