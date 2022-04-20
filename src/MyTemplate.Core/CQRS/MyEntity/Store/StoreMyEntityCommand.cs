using Aertssen.Framework.Infra.CQRS.Base;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Core.CQRS.MyEntity.Store
{
    public class StoreMyEntityCommand : ICommand<Nothing>
    {
        public IStoreMyEntityDto MyEntity { get; set; }
    }
}
