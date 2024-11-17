using SNET.Framework.Domain.Primitives;

namespace SNET.Framework.Domain.Entities
{
    public class Auditory : AggregateRoot
    {
        // Constructor privado para evitar la creación directa sin usar el método Create
        private Auditory(Guid id) : base(id) { }

        private Auditory(
            Guid id,
            string host,
            string description,
            string tenantId = null,
            string userId = null,
            int levelId = 1, // Asignar un valor predeterminado si no se proporciona
            int crudOperationId = 1, // Asignar un valor predeterminado si no se proporciona
            Dictionary<string, object> data = null) : base(id)
        {
            Host = host ?? throw new ArgumentNullException(nameof(host), "El host no puede ser nulo.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "La descripción no puede ser nula.");
            TenantId = tenantId;
            UserId = userId;
            LevelId = levelId;
            CrudOperationId = crudOperationId;
            Data = data ?? new Dictionary<string, object>();
            Created = DateTime.UtcNow;
        }

        // Propiedades
        public string Host { get; private set; }
        public string Description { get; private set; }
        public string TenantId { get; private set; }
        public string UserId { get; private set; }
        public int LevelId { get; private set; }
        public int CrudOperationId { get; private set; }
        public DateTime Created { get; private set; }
        public Dictionary<string, object> Data { get; private set; }

        // Relaciones virtuales
        public virtual CrudOperation CrudOperation { get; set; }
        public virtual Level Level { get; set; }

        // Método estático para la creación centralizada
        public static Auditory Create(
            Guid id,
            string host,
            string description,
            string tenantId = null,
            string userId = null,
            int levelId = 1,
            int crudOperationId = 1,
            Dictionary<string, object> data = null)
        {
            // Validar que el host no sea nulo ni vacío
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("El host no puede ser nulo o vacío.", nameof(host));

            // Validar que la descripción no sea nula ni vacía
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("La descripción no puede ser nula o vacía.", nameof(description));

            // Crear y retornar la instancia
            return new Auditory(
                id,
                host,
                description,
                tenantId,
                userId,
                levelId,
                crudOperationId,
                data
            );
        }
    }
}
