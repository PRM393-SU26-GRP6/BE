using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Discounts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/discounts")]
[Authorize]
public class DiscountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiscountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("validate")]
    [ProducesResponseType(typeof(ValidateDiscountResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidateDiscountResponseDto>> ValidateDiscount([FromBody] ValidateDiscountRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ValidateDiscountCommand(request), cancellationToken);
        return Ok(result);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(IEnumerable<DiscountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetMyDiscounts(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerDiscountsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DiscountDto>> GetDiscount(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDiscountByIdQuery(id, GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DiscountDto>> CreateDiscount([FromBody] DiscountDto discount, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDiscountCommand(GetCurrentUserId(), discount), cancellationToken);
        return CreatedAtAction(nameof(GetDiscount), new { id = result.DiscountId }, result);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DiscountDto>> UpdateDiscount(Guid id, [FromBody] DiscountDto discount, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDiscountCommand(id, GetCurrentUserId(), discount), cancellationToken);
        return Ok(result);
    }

    [NonAction]
    [Authorize(Roles = "Owner,Admin")]
    public async Task<IActionResult> DeleteDiscount(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteDiscountCommand(id, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
    }
}
