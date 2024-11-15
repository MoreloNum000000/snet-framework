using Carter;
using MediatR;
using SNET.Framework.Domain.Shared;
using SNET.Framework.Features.Users.Commands;
using SNET.Framework.Features.Users.Commands.AssignRole;
using SNET.Framework.Features.Users.Commands.DeleteUser;
using SNET.Framework.Features.Users.Commands.RemoveRole;

namespace SNET.Framework.Api.EndPoints
{
    public class UserEndPoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var usersRoute = app.MapGroup("Users");

            usersRoute.MapPost("/", async (CreateUserCommand command, IMediator mediator) =>
            {
                var res = await mediator.Send(command);

                if (res.IsSuccess)
                {
                    return Results.Ok(res);
                }
                else
                {
                    return Results.BadRequest(res);
                }
            })
            .WithName("CreateUser")
            .WithTags("Users")
            .Produces<Result>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);


            usersRoute.MapPost("/Role", async (AssignRoleToUserCommand command, IMediator mediator) =>
            {
                var res = await mediator.Send(command);

                if (res.IsSuccess)
                {
                    return Results.Ok(res);
                }
                else
                {
                    return Results.BadRequest(res);
                }
            })
            .WithName("AssignRoleToUser")
            .WithTags("Users")
            .Produces<Result>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();

            usersRoute.MapDelete("/{userId}", async (Guid userId, IMediator mediator) =>
            {
                // Crear el comando de eliminación del usuario, que usará el userId.
                var command = new DeleteUserCommand(userId);

                // Enviar el comando a través del mediador.
                var res = await mediator.Send(command);

                // Verificar si la operación fue exitosa y retornar el resultado adecuado.
                if (res.IsSuccess)
                {
                    return Results.Ok(res);  // Retorna un 200 OK si fue exitoso.
                }
                else
                {
                    return Results.BadRequest(res);  // Retorna un 400 BadRequest si algo salió mal.
                }
            })
            .WithName("DeleteUser")
            .WithTags("Users")
            .Produces<Result>(StatusCodes.Status200OK)  
            .Produces(StatusCodes.Status400BadRequest)  
            .RequireAuthorization();  


            usersRoute.MapDelete("/Role", async (Guid userId, Guid roleId, IMediator mediator) =>
            {
                var coomand = new RemoveRoleToUserCommand(userId, roleId);
                var res = await mediator.Send(coomand);

                if (res.IsSuccess)
                {
                    return Results.Ok(res);
                }
                else
                {
                    return Results.BadRequest(res);
                }
            })
            .WithName("RemoveRoleToUser")
            .WithTags("Users")
            .Produces<Result>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
        }
    }
}
