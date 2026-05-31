using MediatR;

namespace CourtManager.Application.Features.Fields.Commands;

/// <summary>
/// Toggles a football field's active status (on/off).
/// Only the owner of the parent venue may change the field's status.
/// </summary>
public record UpdateFieldStatusCommand(
    Guid FieldId,
    Guid OwnerId,
    bool IsActive
) : IRequest<UpdateFieldStatusResult>;

public record UpdateFieldStatusResult(
    Guid FieldId,
    bool IsActive,
    string Message
);
