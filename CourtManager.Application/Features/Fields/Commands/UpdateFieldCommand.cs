using CourtManager.Application.DTOs;
using CourtManager.Domain.Enums;
using MediatR;

namespace CourtManager.Application.Features.Fields.Commands;

/// <summary>
/// Updates a football field's name, type, and price.
/// Only the owner of the parent venue may update its fields.
/// </summary>
public record UpdateFieldCommand(
    Guid FieldId,
    Guid OwnerId,
    string FieldName,
    FieldType FieldType,
    decimal PricePerHour
) : IRequest<FootballFieldDto>;
