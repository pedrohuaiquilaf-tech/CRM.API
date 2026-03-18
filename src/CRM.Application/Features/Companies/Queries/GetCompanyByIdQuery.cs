namespace CRM.Application.Features.Companies.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetCompanyByIdQuery(Guid Id) : IRequest<CompanyDto?>;
