using System.Threading.Tasks;
using Aertssen.Framework.Api.Contracts;
using Aertssen.Framework.Core.Validation;
using Aertssen.Framework.Infra.CQRS.Base;
using Aertssen.Framework.Services.Cqrs.Base;
using AutoMapper;
using MediatR;
using MyTemplate.Api.Contracts.Requests.MyEntity;
using MyTemplate.Api.Contracts.Services;
using MyTemplate.Core.CQRS.MyEntity.Store;

namespace MyTemplate.Services.Cqrs
{
    public interface IMyEntityCqrsService : IMyEntityService
    {

    }
    public class MyEntityCqrsService : CqrsService, IMyEntityCqrsService
    {
        public MyEntityCqrsService(IMediator mediator, IMapper mapper, IValidationBag validationBag) 
            : base(mediator, mapper, validationBag)
        {
        }

        public Task<AertssenResponse> StoreMyEntity(StoreMyEntityRequest request)
        {
            return base.Command<StoreMyEntityRequest, StoreMyEntityCommand, Nothing, AertssenResponse>(request);
        }
    }
}
