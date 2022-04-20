using Aertssen.Framework.Infra.CQRS.Base;

namespace MyTemplate.Core.CQRS.MyEntity.Get
{
    public class GetMyEntityQuery : IQuery<GetMyEntityViewModel>
    {
        public long Id { get; set; }
    }
}
