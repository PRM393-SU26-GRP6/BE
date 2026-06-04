using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CourtManager.Application.Features.Auth.Queries;

public class GetMeQueryHandler : IRequestHandler<GetMeQuery, UserDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public GetMeQueryHandler(
        UserManager<User> userManager,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _userManager = userManager;
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

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User not found or inactive.");
        }

        return _mapper.Map<UserDto>(user);
    }
}
