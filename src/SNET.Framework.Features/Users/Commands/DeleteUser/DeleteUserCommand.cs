using MediatR;
using SNET.Framework.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNET.Framework.Features.Users.Commands.DeleteUser
{
    public record DeleteUserCommand(Guid UserId) : IRequest<Result>;

}
