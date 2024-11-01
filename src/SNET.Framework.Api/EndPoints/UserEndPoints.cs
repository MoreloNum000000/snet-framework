﻿using Carter;
using MediatR;
using SNET.Framework.Domain.Audit;
using SNET.Framework.Domain.Shared;
using SNET.Framework.Features.Users.Commands;
using SNET.Framework.Features.Users.Commands.AssignRole;
using SNET.Framework.Features.Users.Commands.RemoveRole;

namespace SNET.Framework.Api.EndPoints
{
    public class UserEndPoints : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {

            app.MapPost("Users", async (CreateUserCommand command, IMediator mediator) =>
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
            .Produces(StatusCodes.Status400BadRequest)
            .WithMetadata(new AuditDescriptionAttribute("Crear un nuevo usuario"));


            app.MapPost("Users/Role", async (AssignRoleToUserCommand command, IMediator mediator) =>
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
            .Produces(StatusCodes.Status400BadRequest);


            app.MapDelete("Users/Role", async (Guid userId, Guid roleId, IMediator mediator) =>
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
            .Produces(StatusCodes.Status400BadRequest);
        }
    }
}
