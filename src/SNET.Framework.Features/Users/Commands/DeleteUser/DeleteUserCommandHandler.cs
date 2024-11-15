using MediatR;
using SNET.Framework.Domain.Repositories;
using SNET.Framework.Domain.Shared;
using SNET.Framework.Domain.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNET.Framework.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Buscar al usuario en el repositorio
            var user = await _userRepository.GetByIdAsync(request.UserId);

            // Si el usuario no existe, devolver un error
            if (user == null)
            {
                return Result.Failure(new Error("DeleteUser.UserNotFound", "Usuario no encontrado"));
            }

            // Eliminar el usuario
            _userRepository.Delete(user);

            // Guardar los cambios en la base de datos
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Devolver un resultado exitoso
            return Result.Success("Usuario eliminado correctamente");
        }
    }

}
