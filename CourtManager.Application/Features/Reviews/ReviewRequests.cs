using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using CourtManager.Domain.Enums;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Reviews;

public record GetReviewsByVenueQuery(Guid VenueId, int PageNumber, int PageSize) : IRequest<IEnumerable<ReviewDto>>;
public record GetReviewsByFieldQuery(Guid FieldId, int PageNumber, int PageSize) : IRequest<IEnumerable<ReviewDto>>;
public record GetReviewByIdQuery(Guid ReviewId) : IRequest<ReviewDto>;
public record GetAverageVenueRatingQuery(Guid VenueId) : IRequest<object>;
public record GetMyReviewsQuery(Guid UserId) : IRequest<IEnumerable<ReviewDto>>;
public record GetUserReviewForBookingQuery(Guid UserId, Guid BookingId) : IRequest<ReviewDto?>;
public record CreateReviewCommand(Guid UserId, ReviewDto Review) : IRequest<ReviewDto>;
public record UpdateReviewCommand(Guid UserId, Guid ReviewId, ReviewDto Review) : IRequest<ReviewDto>;

public class GetReviewsByVenueQueryHandler : IRequestHandler<GetReviewsByVenueQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewsByVenueQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByVenueQuery request, CancellationToken cancellationToken)
    {
        var (reviews, _, _) = await _reviewRepository.GetVenueReviewsAsync(request.VenueId, request.PageNumber, request.PageSize, cancellationToken);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }
}

public class GetReviewsByFieldQueryHandler : IRequestHandler<GetReviewsByFieldQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewsByFieldQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(GetReviewsByFieldQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetReviewsByFieldIdAsync(request.FieldId, request.PageNumber, request.PageSize, cancellationToken);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews);
    }
}

public class GetReviewByIdQueryHandler : IRequestHandler<GetReviewByIdQuery, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetReviewByIdQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(GetReviewByIdQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review == null)
            throw new NotFoundException(nameof(Review), request.ReviewId);

        return _mapper.Map<ReviewDto>(review);
    }
}

public class GetAverageVenueRatingQueryHandler : IRequestHandler<GetAverageVenueRatingQuery, object>
{
    private readonly IReviewRepository _reviewRepository;

    public GetAverageVenueRatingQueryHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<object> Handle(GetAverageVenueRatingQuery request, CancellationToken cancellationToken)
    {
        var result = await _reviewRepository.GetVenueReviewsAsync(request.VenueId, 1, 1, cancellationToken);
        return new { venueId = request.VenueId, averageRating = result.AverageRating };
    }
}

public class GetMyReviewsQueryHandler : IRequestHandler<GetMyReviewsQuery, IEnumerable<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetMyReviewsQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReviewDto>> Handle(GetMyReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<ReviewDto>>(reviews.Where(r => r.UserId == request.UserId).OrderByDescending(r => r.CreatedAt));
    }
}

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(IBookingRepository bookingRepository, IReviewRepository reviewRepository, IMapper mapper)
    {
        _bookingRepository = bookingRepository;
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        if (request.Review.Rating is < 1 or > 5)
            throw new ValidationException("Rating must be between 1 and 5.");

        var booking = await _bookingRepository.GetByIdAsync(request.Review.BookingId, cancellationToken);
        if (booking == null)
            throw new NotFoundException(nameof(Booking), request.Review.BookingId);
        if (booking.UserId != request.UserId)
            throw new ValidationException("Only the booking customer can review this booking.");
        if (booking.BookingStatus != BookingStatus.Completed)
            throw new ValidationException("Only completed bookings can be reviewed.");

        var venueId = request.Review.VenueId != Guid.Empty
            ? request.Review.VenueId
            : booking.BookingItems.Select(i => i.Slot?.Field?.VenueId ?? Guid.Empty).FirstOrDefault(id => id != Guid.Empty);

        if (venueId == Guid.Empty)
            throw new ValidationException("Cannot determine venue for this booking.");

        if (await _reviewRepository.GetReviewByBookingIdAsync(booking.Id, cancellationToken) != null)
            throw new ValidationException("This booking has already been reviewed.");

        var review = new Review
        {
            ReviewId = Guid.NewGuid(),
            UserId = request.UserId,
            VenueId = venueId,
            BookingId = booking.Id,
            Rating = request.Review.Rating,
            Comment = request.Review.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);

        var loaded = await _reviewRepository.GetByIdAsync(review.ReviewId, cancellationToken) ?? review;
        return _mapper.Map<ReviewDto>(loaded);
    }
}

public class GetUserReviewForBookingQueryHandler : IRequestHandler<GetUserReviewForBookingQuery, ReviewDto?>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public GetUserReviewForBookingQueryHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto?> Handle(GetUserReviewForBookingQuery request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetReviewByBookingIdAsync(request.BookingId, cancellationToken);
        return review == null || review.UserId != request.UserId ? null : _mapper.Map<ReviewDto>(review);
    }
}

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IMapper _mapper;

    public UpdateReviewCommandHandler(IReviewRepository reviewRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _reviewRepository.GetByIdAsync(request.ReviewId, cancellationToken);
        if (review == null)
            throw new NotFoundException(nameof(Review), request.ReviewId);
        if (review.UserId != request.UserId)
            throw new ValidationException("Only the review author can update this review.");
        if (request.Review.Rating is < 1 or > 5)
            throw new ValidationException("Rating must be between 1 and 5.");

        review.Rating = request.Review.Rating;
        review.Comment = request.Review.Comment;
        await _reviewRepository.UpdateAsync(review, cancellationToken);
        await _reviewRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<ReviewDto>(review);
    }
}
