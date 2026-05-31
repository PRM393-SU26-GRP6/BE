using AutoMapper;
using CourtManager.Application.DTOs;
using CourtManager.Domain.Interfaces;
using MediatR;

namespace CourtManager.Application.Features.FootballFields.Queries;

public class GetFieldByIdQueryHandler : IRequestHandler<GetFieldByIdQuery, FootballFieldDto>
{
    private readonly IFootballFieldRepository _fieldRepository;
    private readonly IMapper _mapper;

    public GetFieldByIdQueryHandler(IFootballFieldRepository fieldRepository, IMapper mapper)
    {
        _fieldRepository = fieldRepository;
        _mapper = mapper;
    }

    public async Task<FootballFieldDto> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        var field = await _fieldRepository.GetByIdAsync(request.FieldId, cancellationToken);
        
        if (field == null)
            throw new Exception("Field not found");

        return _mapper.Map<FootballFieldDto>(field);
    }
}
