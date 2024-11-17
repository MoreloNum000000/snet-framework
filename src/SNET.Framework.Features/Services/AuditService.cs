using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SNET.Framework.Domain;
using SNET.Framework.Domain.Entities;
using SNET.Framework.Domain.Repositories;
using SNET.Framework.Domain.Shared;
using SNET.Framework.Domain.UnitOfWork;
using System.Net;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(IAuditoryRepository repository, IUnitOfWork unitOfWork, ILogger<AuditService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _auditoryRepository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
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
                // Log the received model
                _logger.LogInformation("Modelo de auditoría recibido: {Model}", JsonConvert.SerializeObject(model));

                // Obtener la IP del cliente desde el contexto HTTP
                var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";
                string clientHost = "Unknown Host";

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    try
                    {
                        var hostEntry = Dns.GetHostEntry(ipAddress);
                        clientHost = $"{hostEntry.HostName} ({ipAddress})";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Error al obtener el nombre del host para la IP {ipAddress}: {ex.Message}");
                        clientHost = ipAddress;
                    }
                }

                model.Host ??= clientHost;

                // Obtener las claims del usuario autenticado
                var claims = _httpContextAccessor.HttpContext?.User?.Claims.ToList();
                if (claims != null)
                {
                    _logger.LogInformation("Claims del usuario autenticado:");
                    foreach (var claim in claims)
                    {
                        _logger.LogInformation("Claim Type: {Type}, Claim Value: {Value}", claim.Type, claim.Value);
                    }
                }

                // Filtrar claims del tipo "nameidentifier" y buscar el que sea un GUID válido
                var nameIdentifierClaims = claims?
                    .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                    .ToList();

                if (nameIdentifierClaims != null && nameIdentifierClaims.Any())
                {
                    var userIdClaim = nameIdentifierClaims.FirstOrDefault(c => Guid.TryParse(c.Value, out _))?.Value;

                    if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
                    {
                        _logger.LogInformation("UserId obtenido desde las claims: {UserId}", userId);

                        // Crear la entidad de auditoría utilizando el método Create
                        var auditory = Auditory.Create(
                            Guid.NewGuid(),
                            userId, // UserId del usuario actual
                            model.Host,
                            model.Description,
                            model.TenantId,
                            model.TableName,
                            (int)model.LevelId,
                            (int)model.CrudOperationId,
                            model.Data
                        );

                        // Persistir la auditoría en la base de datos
                        _auditoryRepository.Add(auditory);
                        await _unitOfWork.SaveChangesAsync(cancellationToken);

                        _logger.LogInformation("Auditoría registrada: {Description}", model.Description);

                        return Result.Success("Auditoría creada correctamente.");
                    }
                }

                _logger.LogWarning("No se encontró un UserId válido en las claims.");
                return Result.Failure(new Error("CreateAudit.UserNotAuthenticated", "Usuario no autenticado o UserId no válido."));
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
        public string TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Host { get; set; }
        public string Description { get; set; }
        public string TableName { get; set; }
        public AuditLevel LevelId { get; set; }
        public AuditCrudOperation CrudOperationId { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
