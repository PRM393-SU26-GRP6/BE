
using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Application.Exceptions;
using CourtManager.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;


namespace CourtManager.Application.Features.Auth.Queries
{
    public class GetMeHandler : IRequestHandler<GetMeQueries, UserDto>
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        public GetMeHandler(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        public async Task<UserDto> Handle(GetMeQueries request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
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
