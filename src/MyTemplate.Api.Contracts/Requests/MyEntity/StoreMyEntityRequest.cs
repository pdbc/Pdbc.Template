using Aertssen.Framework.Api.Contracts;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Api.Contracts.Requests.MyEntity
{
    public class StoreMyEntityRequest : AertssenRequest
    {
        public StoreMyEntityDto MyEntity { get; set; }
    }
}
