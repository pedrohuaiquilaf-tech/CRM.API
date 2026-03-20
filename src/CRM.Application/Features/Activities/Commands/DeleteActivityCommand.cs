namespace CRM.Application.Features.Activities.Commands;

using MediatR;

public record DeleteActivityCommand(Guid Id) : IRequest<bool>;
