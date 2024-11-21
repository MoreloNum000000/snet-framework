using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using SNET.Framework.Domain.Entities;

namespace SNET.Framework.Persistence.Configuraciones
{
    internal class AuditoryConfiguration : IEntityTypeConfiguration<Auditory>
    {
        public void Configure(EntityTypeBuilder<Auditory> builder)
        {
            builder.HasKey(e => e.Id);

            builder.ToTable("Auditories");

            builder.Property(e => e.Host)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.HasOne(e => e.Level)  
                   .WithMany(l => l.Auditory)  
                   .HasForeignKey(e => e.LevelId)
                   .OnDelete(DeleteBehavior.Restrict);  

            builder.HasOne(e => e.CrudOperation) 
                   .WithMany(c => c.Auditory)  
                   .HasForeignKey(e => e.CrudOperationId)
                   .OnDelete(DeleteBehavior.Restrict);  

            // Convertir enums a int para la base de datos (si aplica, por ejemplo, si LevelId o CrudOperationId son enums)
            builder.Property(e => e.LevelId)
                   .IsRequired()
                   .HasConversion<int>(); 

            builder.Property(e => e.CrudOperationId)
                   .IsRequired()
                   .HasConversion<int>(); 

            // Configuración para propiedades complejas como JSON (Data)
            builder.Property(e => e.Data)
                   .HasConversion(
                       v => JsonConvert.SerializeObject(v), // Serializar al guardar
                       v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v)) // Deserializar al leer
                   .HasColumnType("nvarchar(max)"); 

            builder.Property(e => e.Created)
                   .IsRequired();
        }
    }
}
