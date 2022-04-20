using System.Threading;
using Aertssen.Framework.Infra.CQRS.Base;
using Aertssen.Framework.Tests;
using Aertssen.Framework.Tests.Extensions;
using NUnit.Framework;

namespace MyTemplate.UnitTests.Core.CQRS.Reservations.Place
{
    [TestFixture]
    public class PlaceReservationCommandHandlerSpecification : ContextSpecification<PlaceReservationCommandHandler>
    {
        protected PlaceReservationCommand Command { get; set; }


        protected IReservationRepository Repository => Dependency<IReservationRepository>();
        protected IFactory<IPlaceReservationDto, Reservation> Factory => Dependency<IFactory<IPlaceReservationDto, Reservation>>();
        
        protected Reservation CreatedReservation;

        protected override void Establish_context()
        {
            base.Establish_context();
            Command = new PlaceReservationCommandTestDataBuilder()
                .Build();
            
            CreatedReservation = new ReservationTestDataBuilder();

            Factory.Stub(x => x.Create(Command.Reservation)).Return(CreatedReservation);
        }

        protected override void Because()
        {
            SUT.Handle(Command, new CancellationToken()).GetAwaiter().GetResult();
        }

        [Test]
        public void Verify_factory_called_to_update_the_object()
        {
            Factory.AssertWasCalled(x => x.Create(Command.Reservation));
        }

        [Test]
        public void Verify_repository_called_to_insert_the_object()
        {
            Repository.AssertWasCalled(x => x.Insert(CreatedReservation));
        }
    }
}