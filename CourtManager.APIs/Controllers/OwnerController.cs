using CourtManager.Application.DTOs;
using CourtManager.Application.Features.Bookings.Commands;
using CourtManager.Application.Features.Bookings.Queries;
using CourtManager.Application.Features.Discounts;
using CourtManager.Application.Features.Fields;
using CourtManager.Application.Features.Owner;
using CourtManager.Application.Features.TimeSlots;
using CourtManager.Application.Features.TimeSlots.Commands;
using CourtManager.Application.Features.Venues.Commands;
using CourtManager.Application.Features.Venues.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CourtManager.APIs.Controllers;

[ApiController]
[Route("api/v1/owner")]
[Authorize(Roles = "Owner,Admin")]
public class OwnerController : ControllerBase
{
    private readonly IMediator _mediator;

    public OwnerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("stats")]
    [ProducesResponseType(typeof(OwnerStatsDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<OwnerStatsDto>> GetStats(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerStatsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> GetRevenue([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string groupBy = "day", CancellationToken cancellationToken = default)
    {
        var data = await _mediator.Send(new GetOwnerRevenueQuery(GetCurrentUserId(), from, to, groupBy), cancellationToken);
        return Ok(data);
    }



    [HttpPost("venues/{id:guid}/amenities")]
    public async Task<IActionResult> AddVenueAmenities(Guid id, [FromBody] VenueAmenityRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AddVenueAmenitiesCommand(GetCurrentUserId(), id, request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("venues/{id:guid}/amenities/{amenityId:guid}")]
    public async Task<IActionResult> DeleteVenueAmenity(Guid id, Guid amenityId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteVenueAmenityCommand(GetCurrentUserId(), id, amenityId), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpGet("venues/{venueId}/fields")]
    [ProducesResponseType(typeof(IEnumerable<FootballFieldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FootballFieldDto>>> GetFields(Guid venueId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetFieldsByVenueQuery(venueId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("venues/{venueId}/fields")]
    [ProducesResponseType(typeof(FootballFieldDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<FootballFieldDto>> CreateField(Guid venueId, [FromBody] FootballFieldDto field, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateFieldCommand(GetCurrentUserId(), venueId, field), cancellationToken);
        return Created($"/api/v1/fields/{result.Id}", result);
    }



    [HttpPost("fields/{id:guid}/slots/bulk")]
    public async Task<IActionResult> BulkCreateSlots(Guid id, [FromBody] BulkCreateSlotsDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new BulkCreateSlotsCommand(GetCurrentUserId(), id, request), cancellationToken);
        return Ok(result);
    }

    [HttpPut("slots/{id:guid}")]
    public async Task<ActionResult<TimeSlotDto>> UpdateSlot(Guid id, [FromBody] TimeSlotDto slot, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateTimeSlotCommand(GetCurrentUserId(), id, slot), cancellationToken);
        return Ok(result);
    }

    [HttpPut("slots/{id:guid}/status")]
    public async Task<IActionResult> UpdateSlotStatus(Guid id, [FromBody] UpdateSlotStatusDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateSlotStatusCommand(GetCurrentUserId(), id, request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("slots/{id:guid}")]
    public async Task<IActionResult> DeleteSlot(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteTimeSlotCommand(id), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpGet("bookings/pending")]
    [ProducesResponseType(typeof(IEnumerable<BookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetPendingBookings(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerPendingBookingsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookings(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerBookingsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpGet("bookings/{id:guid}")]
    public async Task<ActionResult<BookingDto>> GetBooking(Guid id, CancellationToken cancellationToken)
    {
        var bookings = await _mediator.Send(new GetOwnerBookingsQuery(GetCurrentUserId()), cancellationToken);
        var booking = bookings.FirstOrDefault(b => b.Id == id);
        return booking == null ? NotFound() : Ok(booking);
    }

    [HttpPut("bookings/{id:guid}/accept")]
    public async Task<IActionResult> AcceptBooking(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new AcceptBookingCommand(id, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpPut("bookings/{id:guid}/reject")]
    public async Task<IActionResult> RejectBooking(Guid id, [FromQuery] string? rejectionReason, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RejectBookingCommand(id, rejectionReason, GetCurrentUserId()), cancellationToken);
        return Ok(new { success = result });
    }

    [HttpPut("bookings/{id:guid}/complete")]
    public async Task<IActionResult> CompleteBooking(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteBookingCommand(GetCurrentUserId(), id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("discounts")]
    [ProducesResponseType(typeof(IEnumerable<DiscountDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetDiscounts(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOwnerDiscountsQuery(GetCurrentUserId()), cancellationToken);
        return Ok(result);
    }

    [HttpPost("discounts")]
    [ProducesResponseType(typeof(DiscountDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<DiscountDto>> CreateDiscount([FromBody] DiscountDto discount, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDiscountCommand(GetCurrentUserId(), discount), cancellationToken);
        return Created($"/api/v1/owner/discounts/{result.DiscountId}", result);
    }

    [HttpPut("discounts/{id:guid}")]
    public async Task<ActionResult<DiscountDto>> UpdateDiscount(Guid id, [FromBody] DiscountDto discount, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDiscountCommand(id, GetCurrentUserId(), discount), cancellationToken);
        return Ok(result);
    }

    [HttpPut("discounts/{id:guid}/status")]
    public async Task<IActionResult> UpdateDiscountStatus(Guid id, [FromBody] UpdateStatusDto request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDiscountStatusCommand(GetCurrentUserId(), id, request), cancellationToken);
        return Ok(result);
    }

    [HttpDelete("discounts/{id:guid}")]
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
