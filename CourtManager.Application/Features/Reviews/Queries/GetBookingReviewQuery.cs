using CourtManager.Application.DTOs;
using MediatR;

namespace CourtManager.Application.Features.Reviews.Queries;

public record GetBookingReviewQuery(Guid BookingId) : IRequest<ReviewDto?>;
