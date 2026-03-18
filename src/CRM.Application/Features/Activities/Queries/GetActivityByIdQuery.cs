namespace CRM.Application.Features.Activities.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetActivityByIdQuery(Guid Id) : IRequest<ActivityDto?>;
