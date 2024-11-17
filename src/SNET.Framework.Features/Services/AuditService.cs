using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SNET.Framework.Domain;
using SNET.Framework.Domain.Entities;
using SNET.Framework.Domain.Repositories;
using SNET.Framework.Domain.Shared;
using SNET.Framework.Domain.UnitOfWork;

namespace SNET.Framework.Features.Services
{
    public interface IAuditService
    {
        Task<Result> RegisterAuditAsync(CreateAuditModel model, CancellationToken cancellationToken);
    }

    public class AuditService : IAuditService
    {
        private readonly IAuditoryRepository _auditoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuditService> _logger;
        private readonly Microsoft.AspNetCore.Http.IHttpContextAccessor _httpContextAccessor;

        public AuditService(IAuditoryRepository repository, IUnitOfWork unitOfWork, ILogger<AuditService> logger, Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        {
            _auditoryRepository = repository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<Result> RegisterAuditAsync(CreateAuditModel model, CancellationToken cancellationToken)
        {
            if (model == null)
            {
                _logger.LogError("El modelo de auditoría es nulo.");
                return Result.Failure(new Error("CreateAudit.ValidationError", "El modelo de auditoría es nulo"));
            }

            try
            {
                // Verifica los valores del modelo de auditoría
                _logger.LogInformation("Modelo de auditoría recibido: {Model}", JsonConvert.SerializeObject(model));

                // Enriquecer el modelo con datos del contexto HTTP
                model.Host ??= _httpContextAccessor.HttpContext?.Request.Host.Value ?? "Unknown";

                // Crear la entidad de auditoría utilizando el método Create
                var auditory = Auditory.Create(
                    Guid.NewGuid(),
                    model.Host,
                    model.Description,
                    model.TenantId,
                    model.UserId,
                    (int)model.LevelId,
                    (int)model.CrudOperationId,
                    model.Data
                );

                // Verifica si la creación de la entidad es exitosa
                _logger.LogInformation("Entidad de auditoría creada: {Auditory}", JsonConvert.SerializeObject(auditory));

                // Persistir la auditoría en la base de datos
                _auditoryRepository.Add(auditory);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Auditoría registrada: {Description}", model.Description);

                return Result.Success("Auditoría creada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría: {Description}", model.Description);
                return Result.Failure(new Error("CreateAudit.ValidationError", "Error al registrar auditoría."));
            }
        }
    }

    public class CreateAuditModel
    {
        public string Host { get; set; }
        public string Description { get; set; }
        public string TenantId { get; set; }
        public string UserId { get; set; }
        public AuditLevel LevelId { get; set; }
        public AuditCrudOperation CrudOperationId { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
