using FluentValidation;
using MediatR;
using SNET.Framework.Domain;
using SNET.Framework.Domain.Entities;
using SNET.Framework.Domain.Repositories;
using SNET.Framework.Domain.Shared;
using SNET.Framework.Domain.UnitOfWork;
using SNET.Framework.Features.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SNET.Framework.Features.Users.Commands
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CreateUserCommand> _validator;
        private readonly IAuditService _auditService;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IValidator<CreateUserCommand> validator,
            IAuditService auditService
            )
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _auditService = auditService;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Validación del comando
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                return Result.Failure(new Error("CreateUser.ValidationError", "Datos no válidos"));
            }

            // Crear el usuario
            var user = User.Create(
                request.Id,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                request.Password);

            // Agregar el usuario al repositorio
            _userRepository.Add(user);

            // Guardar cambios en la base de datos
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Preparar el modelo para la auditoría (sin necesidad de pasar el UserId)
            var auditModel = new CreateAuditModel
            {
                Description = $"Usuario creado: {user.Email}",
                TenantId = "tenant123", // Ajustar según corresponda
                TableName = "Users",
                LevelId = AuditLevel.Information, // Enum para el nivel de auditoría
                CrudOperationId = AuditCrudOperation.Insert,
                Data = new Dictionary<string, object>
                {
                    { "UserId", user.Id },
                    { "Email", user.Email }
                }
            };

            // Registrar la auditoría
            var auditResult = await _auditService.RegisterAuditAsync(auditModel, cancellationToken);
            if (!auditResult.IsSuccess)
            {
                return Result.Failure(new Error("CreateUser.AuditFailure", "No se pudo registrar la auditoría"));
            }

            // Retornar éxito
            return Result.Success("Usuario creado correctamente");
        }
    }
}
