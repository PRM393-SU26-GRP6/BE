using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Entities;

namespace CourtManager.Application.Mappings;

/// <summary>
/// AutoMapper profile for entity to DTO mappings.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User mappings
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Amenity, AmenityDto>().ReverseMap();

        // FootballField mappings
        CreateMap<FootballField, FootballFieldDto>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Venue != null ? src.Venue.OwnerId : Guid.Empty))
            .ForMember(dest => dest.FieldType, opt => opt.MapFrom(src => src.FieldType.ToString()))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Venue != null ? src.Venue.Address : string.Empty))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Venue != null ? src.Venue.Latitude : 0))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Venue != null ? src.Venue.Longitude : 0))
            .ReverseMap();

        // Venue mappings
        CreateMap<Venue, VenueDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner != null ? src.Owner.FullName : string.Empty))
            .ForMember(dest => dest.TotalReviews, opt => opt.MapFrom(src => src.Reviews.Count))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => 
                src.Reviews.Any() ? Math.Round(src.Reviews.Average(r => r.Rating), 1) : 0))
            .ForMember(dest => dest.MinPrice, opt => opt.MapFrom(src => 
                src.FootballFields.Any() ? src.FootballFields.Min(f => f.PricePerHour) : 0))
            .ForMember(dest => dest.MaxPrice, opt => opt.MapFrom(src => 
                src.FootballFields.Any() ? src.FootballFields.Max(f => f.PricePerHour) : 0));

        CreateMap<Venue, VenueDetailDto>()
            .IncludeBase<Venue, VenueDto>()
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.VenueImages))
            .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.VenueAmenities.Select(va => va.Amenity)))
            .ForMember(dest => dest.Fields, opt => opt.MapFrom(src => src.FootballFields));

        CreateMap<VenueImage, VenueImageDto>().ReverseMap();
        CreateMap<Amenity, AmenityDto>().ReverseMap();

        // Booking mappings
        CreateMap<BookingItem, BookingItemDto>()
            .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.Slot != null ? src.Slot.FieldId : Guid.Empty))
            .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Slot != null && src.Slot.Field != null ? src.Slot.Field.FieldName : null))
            .ForMember(dest => dest.VenueId, opt => opt.MapFrom(src => src.Slot != null && src.Slot.Field != null ? src.Slot.Field.VenueId : Guid.Empty))
            .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Slot != null && src.Slot.Field != null && src.Slot.Field.Venue != null ? src.Slot.Field.Venue.VenueName : null))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Slot != null ? src.Slot.StartTime : default))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Slot != null ? src.Slot.EndTime : default));
        CreateMap<Booking, BookingDto>()
            .ForMember(dest => dest.FieldId, opt => opt.MapFrom(src => src.BookingItems.Select(i => i.Slot != null ? i.Slot.FieldId : Guid.Empty).FirstOrDefault()))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.BookingItems.Any() ? src.BookingItems.Min(i => i.Slot != null ? i.Slot.StartTime : DateTime.MinValue) : DateTime.MinValue))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.BookingItems.Any() ? src.BookingItems.Max(i => i.Slot != null ? i.Slot.EndTime : DateTime.MinValue) : DateTime.MinValue))
            .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.BookingStatus.ToString()))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.BookingDiscounts.Sum(d => d.DiscountAmount)))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.BookingItems))
            .ForMember(dest => dest.Payments, opt => opt.MapFrom(src => src.Payments))
            .ReverseMap();
        CreateMap<Booking, CreateBookingDto>().ReverseMap();
        CreateMap<Booking, BookingHistoryDto>()
            //.ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.Field != null ? src.Field.FieldName : null))
            //.ForMember(dest => dest.FieldLocation, opt => opt.MapFrom(src => src.Field != null ? src.Field.Location : null))
            //.ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Field != null && src.Field.Owner != null ? src.Field.Owner.FullName : null))
            .ReverseMap();

        // Payment mappings
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.PaymentType, opt => opt.MapFrom(src => src.PaymentType.ToString()))
            .ForMember(dest => dest.BookingStatus, opt => opt.MapFrom(src => src.Booking != null ? src.Booking.BookingStatus.ToString() : null))
            .ReverseMap();

        // TimeSlot mappings
        CreateMap<TimeSlot, TimeSlotDto>()
            .ForMember(dest => dest.SlotStatus, opt => opt.MapFrom(src => src.SlotStatus.ToString()))
            .ReverseMap();

        // FieldImage mappings
        // CreateMap<FieldImage, FieldImageDto>().ReverseMap();

        // ChatRoom mappings
        CreateMap<ChatRoom, ChatRoomDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : null))
            .ForMember(dest => dest.HostName, opt => opt.MapFrom(src => src.Host != null ? src.Host.FullName : null))
            .ReverseMap();

        // Message mappings
        CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender != null ? src.Sender.FullName : null))
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.ReadAt.HasValue));

        // Notification mappings
        CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ReverseMap();

        // Review mappings
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : null))
            .ForMember(dest => dest.VenueName, opt => opt.MapFrom(src => src.Venue != null ? src.Venue.VenueName : null))
            .ReverseMap();

        CreateMap<Discount, DiscountDto>()
            .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => src.DiscountType.ToString()))
            .ReverseMap();
    }
}
