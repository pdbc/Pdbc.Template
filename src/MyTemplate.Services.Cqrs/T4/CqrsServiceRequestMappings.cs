using AutoMapper;

namespace MyTemplate.Services.Cqrs
{
    public class CqrsServiceRequestToCqrsMappings : Profile
    {
        public CqrsServiceRequestToCqrsMappings()
        {
            AddGlobalIgnore("Notifications");
            AddGlobalIgnore("Items");

          
// StoreMyEntityRequest StoreMyEntityCommand GetMyEntityQuery
  
// StoreMyEntityRequest StoreMyEntityCommand GetMyEntityViewModel
  
// StoreMyEntityRequest StoreMyEntityCommand ListMyEntitiesQuery
  
// StoreMyEntityRequest StoreMyEntityCommand StoreMyEntityCommand
            CreateMap<MyTemplate.Api.Contracts.Requests.MyEntity.StoreMyEntityRequest, MyTemplate.Core.CQRS.MyEntity.Store.StoreMyEntityCommand>();
        }
    }
}
