using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MyTemplate.Data.Repositories;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Core.CQRS.MyEntity.Get
{
    public class GetMyEntityQueryHandler : IRequestHandler<GetMyEntityQuery, GetMyEntityViewModel>
    {
        private readonly IMyEntityRepository _repository;
        private readonly IMapper _mapper;

        public GetMyEntityQueryHandler(IMyEntityRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<GetMyEntityViewModel> Handle(GetMyEntityQuery request, CancellationToken cancellationToken)
        {
            return new GetMyEntityViewModel()
            {
                MyEntity = _mapper.Map<MyEntityDataDto>(await _repository.GetByIdAsync(request.Id))
            };
        }
    }
}