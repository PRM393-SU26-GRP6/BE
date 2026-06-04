
using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Application.Interfaces;
using CourtManager.Domain.Entities;
using MediatR;


namespace CourtManager.Application.Features.Auth.Queries
{
    public class GetMeHandler : IRequestHandler<GetMeQueries, UserDto>
    {
        private readonly IUserAuthService _userAuthService;
        private readonly IMapper _mapper;
        public GetMeHandler(IUserAuthService userAuthService, IMapper mapper)
        {
            _userAuthService = userAuthService;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(GetMeQueries request, CancellationToken cancellationToken)
        {
            var user = await _userAuthService.FindByIdAsync(request.UserId);
            if (user == null || !user.IsActive)
            {
                throw new NotFoundException("User not found or inactive.");
            }
            // 2. Map từ Entity (User) sang DTO (UserDto) để giấu bớt các trường nhạy cảm (như password hash)
            var userDto = _mapper.Map<UserDto>(user);

            return userDto;
        }
    }
}
