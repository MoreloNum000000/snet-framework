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
            // Definir la clave primaria
            builder.HasKey(e => e.Id);

            // Definir la tabla en la base de datos
            builder.ToTable("Auditories");

            // Mapear propiedades con restricciones de longitud
            builder.Property(e => e.Host)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(e => e.Description)
                   .IsRequired()
                   .HasMaxLength(500);

            // Configuración para las claves foráneas (LevelId y CrudOperationId)
            builder.HasOne(e => e.Level)  // Relación con la entidad Level
                   .WithMany(l => l.Auditory)  // Un Level puede tener muchas auditorías
                   .HasForeignKey(e => e.LevelId)
                   .OnDelete(DeleteBehavior.Restrict);  // No eliminar auditoría si se elimina un Level

            builder.HasOne(e => e.CrudOperation)  // Relación con la entidad CrudOperation
                   .WithMany(c => c.Auditory)  // Un CrudOperation puede tener muchas auditorías
                   .HasForeignKey(e => e.CrudOperationId)
                   .OnDelete(DeleteBehavior.Restrict);  // No eliminar auditoría si se elimina un CrudOperation

            // Convertir enums a int para la base de datos (si aplica, por ejemplo, si LevelId o CrudOperationId son enums)
            builder.Property(e => e.LevelId)
                   .IsRequired()
                   .HasConversion<int>(); // Convertir enum a int para la base de datos

            builder.Property(e => e.CrudOperationId)
                   .IsRequired()
                   .HasConversion<int>(); // Convertir enum a int para la base de datos

            // Configuración para propiedades complejas como JSON (Data)
            builder.Property(e => e.Data)
                   .HasConversion(
                       v => JsonConvert.SerializeObject(v), // Serializar al guardar
                       v => JsonConvert.DeserializeObject<Dictionary<string, object>>(v)) // Deserializar al leer
                   .HasColumnType("nvarchar(max)"); // Usar "nvarchar(max)" para SQL Server

            // Configurar la fecha de creación
            builder.Property(e => e.Created)
                   .IsRequired();
        }
    }
}
