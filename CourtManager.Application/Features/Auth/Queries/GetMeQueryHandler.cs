using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.Auth.Queries;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, UserDto>
{
    private readonly IUserAuthService _userAuthService;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetMeQueryHandler(
        IUserAuthService userAuthService,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userAuthService = userAuthService;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<UserDto> Handle(GetMeQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException("Invalid token claims or user is not authenticated.");
        }

        var user = await _userAuthService.FindByIdAsync(userId);

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or inactive.");
        }

        return _mapper.Map<UserDto>(user);
    }
}
