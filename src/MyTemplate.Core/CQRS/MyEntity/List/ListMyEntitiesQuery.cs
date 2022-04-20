using System.Linq;
using Aertssen.Framework.Infra.CQRS.Base;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Core.CQRS.MyEntity.List
{
    public class ListMyEntitiesQuery : IQuery<IQueryable<MyEntityDataDto>>
    {
    }
}
