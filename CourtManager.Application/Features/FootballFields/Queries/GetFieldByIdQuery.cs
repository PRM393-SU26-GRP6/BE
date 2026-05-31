using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.FootballFields.Queries;

public record GetFieldByIdQuery(Guid FieldId) : IRequest<FootballFieldDto>;
