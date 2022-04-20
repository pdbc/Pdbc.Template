using System.Threading;
using System.Threading.Tasks;
using Aertssen.Framework.Tests;
using Aertssen.Framework.Tests.Extensions;
using AutoMapper;
using Moq;
using MyTemplate.Core.CQRS.MyEntity.Get;
using MyTemplate.Data.Repositories;
using MyTemplate.Domain.Model;
using MyTemplate.Dto.MyEntity;
using MyTemplate.Tests.Helpers.CQRS.Reservations;
using MyTemplate.Tests.Helpers.Domain;
using NUnit.Framework;

namespace MyTemplate.UnitTests.Core.CQRS.Reservations.Get
{
    [TestFixture]
    public class GetReservationQueryHandlerSpecification : ContextSpecification<GetMyEntityQueryHandler>
    {
        protected GetMyEntityQuery Query { get; set; }
        protected CancellationToken CancellationToken;

        protected MyEntity Entity;

        protected IMyEntityRepository Repository => Dependency<IMyEntityRepository>();
        protected IMapper Mapper => Dependency<IMapper>();

        protected override void Establish_context()
        {
            base.Establish_context();
            Query = new GetMyEntityQueryTestDataBuilder().Build();

            CancellationToken = new CancellationToken();
            Entity = new MyEntityTestDataBuilder().Build();

            Repository.Stub(x => x.GetByIdAsync(Query.Id)).Returns(Task.FromResult(Entity));

        }

        protected override void Because()
        {
            SUT.Handle(Query, CancellationToken).GetAwaiter().GetResult();
        }

        [Test]
        public void Verify_repository_called_to_load_companies()
        {
            Repository.AssertWasCalled(x => x.GetByIdAsync(Query.Id));
        }

        [Test]
        public void Verify_mapper_called()
        {
            Mapper.AssertWasCalled(x => x.Map<MyEntityDataDto>(It.IsAny<object>()));
        }
    }

}