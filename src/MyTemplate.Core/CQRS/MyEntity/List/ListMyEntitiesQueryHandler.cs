using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aertssen.Framework.Infra.Services;
using MediatR;
using MyTemplate.Data.Repositories;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Core.CQRS.MyEntity.List
{
    public class ListMyEntitiesQueryHandler : IRequestHandler<ListMyEntitiesQuery, IQueryable<MyEntityDataDto>>
    {
        private readonly IMyEntityRepository _repository;
        private readonly IProjectionService _projectionService;

        public ListMyEntitiesQueryHandler(IMyEntityRepository repository, IProjectionService projectionService)
        {
            _repository = repository;
            _projectionService = projectionService;
        }

        public async Task<IQueryable<MyEntityDataDto>> Handle(ListMyEntitiesQuery request, CancellationToken cancellationToken)
        {
            var queryable = _repository.GetAll();
            var mapped = _projectionService.Project<Domain.Model.MyEntity, MyEntityDataDto>(queryable);
            return mapped;
        }
    }
}