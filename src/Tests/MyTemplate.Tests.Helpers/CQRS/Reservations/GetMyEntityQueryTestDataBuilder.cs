using MyTemplate.Core.CQRS.MyEntity.Get;
using MyTemplate.Core.T4;

namespace MyTemplate.Tests.Helpers.CQRS.Reservations
{
    public class GetMyEntityQueryTestDataBuilder : GetMyEntityQueryBuilder
    {
        public GetMyEntityQueryTestDataBuilder()
        {
            Id = 1;
        }
    }
}
