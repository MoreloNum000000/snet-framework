using FluentValidation;
using MediatR;
using SNET.Framework.Domain;
using SNET.Framework.Domain.Entities;
using SNET.Framework.Domain.Repositories;
using SNET.Framework.Domain.Shared;
using SNET.Framework.Domain.UnitOfWork;
using SNET.Framework.Features.Services;

namespace SNET.Framework.Features.Users.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateUserCommand> _validator;
    private readonly IAuditService _auditService;

    public CreateUserCommandHandler(
        IUserRepository repository,
        IUnitOfWork unitOfWork,
        IValidator<CreateUserCommand> validator,
        IAuditService auditService)
    {
        _userRepository = repository;
        _unitOfWork = unitOfWork;
        _validator = validator;
        _auditService = auditService;
    }

    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Validar el comando
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

        _userRepository.Add(user);

        // Guardar los cambios en la base de datos
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Registrar la acción de auditoría
        var auditModel = new CreateAuditModel
        {
            Host = "localhost", // Obtener el host real, por ejemplo, desde HttpContext
            Description = $"Usuario creado: {user.Email}",
            TenantId = "tenant123", // Agregar TenantId si aplica
            UserId = user.Id.ToString(),
            LevelId = AuditLevel.Information, // Usar el valor entero del enum
            CrudOperationId = AuditCrudOperation.Insert,
            Data = new Dictionary<string, object>
            {
                { "UserId", user.Id },
                { "Email", user.Email }
            }
        };

        var auditResult = await _auditService.RegisterAuditAsync(auditModel, cancellationToken);
        if (!auditResult.IsSuccess)
        {
            return Result.Failure(new Error("CreateUser.AuditFailure", "No se pudo registrar la auditoría"));
        }

        return Result.Success("Usuario creado correctamente");
    }
}
