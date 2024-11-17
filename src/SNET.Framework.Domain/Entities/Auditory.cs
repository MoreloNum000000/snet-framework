using SNET.Framework.Domain.Primitives;

namespace SNET.Framework.Domain.Entities
{
    public class Auditory : AggregateRoot
    {
        // Constructor privado para evitar la creación directa sin usar el método Create
        private Auditory(Guid id) : base(id) { }

        private Auditory(
            Guid id,
            Guid userId,
            string host,
            string description,
            string tableName,
            string tenantId = null,
            int levelId = 1, 
            int crudOperationId = 1, 
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
            TableName = tableName ?? throw new ArgumentNullException(nameof(tableName), "El tableName no puede ser nulo."); ;
        }

        // Propiedades
        public string Host { get; private set; }
        public string Description { get; private set; }
        public string TenantId { get; private set; }
        public Guid UserId { get; private set; }
        public int LevelId { get; private set; }
        public int CrudOperationId { get; private set; }
        public DateTime Created { get; private set; }
        public Dictionary<string, object> Data { get; private set; }
        public string TableName { get; set; }


        // Relaciones virtuales
        public virtual CrudOperation CrudOperation { get; set; }
        public virtual Level Level { get; set; }




        // Método estático para la creación centralizada
        public static Auditory Create(
            Guid id,
            Guid userId,
            string host,
            string description,
            string tableName,
            string tenantId = null,
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
                userId,
                host,
                description,
                tenantId,
                tableName,
                levelId,
                crudOperationId,
                data
            );
        }
    }
}
