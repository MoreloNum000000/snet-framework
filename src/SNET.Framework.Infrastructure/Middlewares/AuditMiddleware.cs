using Microsoft.AspNetCore.Http;
using SNET.Framework.Domain.Audit;


namespace SNET.Framework.Infrastructure.Middlewares
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            var userId = Guid.NewGuid(); 
            var userName = "AdminTest"; 

            Console.WriteLine("Inicio del middleware de auditoría");

            var method = context.Request.Method;
            var path = context.Request.Path;

            await _next(context);

            var endpoint = context.GetEndpoint();
            string actionDescription = "Descripción de acción no disponible"; // Valor predeterminado
            if (endpoint != null)
            {
                var auditDescriptionAttribute = endpoint.Metadata.GetMetadata<AuditDescriptionAttribute>();
                if (auditDescriptionAttribute != null)
                {
                    actionDescription = auditDescriptionAttribute.Description;
                }
            }

            await auditService.LogCreateAsync(new AuditLog
            {
                UserId = userId,
                UserName = userName,
                TableName = path.ToString(),
                Timestamp = DateTime.UtcNow,
                OldValue = null,
                NewValue = $"Acción realizada: {actionDescription}",
                ChangeReason = $"{method} en {path}"
            });
        }
    }
}
