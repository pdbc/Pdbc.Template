using Aertssen.Framework.Infra.CQRS.Base;
using Aertssen.Framework.Tests;
using Aertssen.Framework.Tests.Extensions;
using Moq;
using NUnit.Framework;

namespace MyTemplate.UnitTests.Core.CQRS.Reservations.Place
{
    [TestFixture]
    public class PlaceReservationFactorySpecification : ContextSpecification<PlaceReservationFactory>
    {
        protected PlaceReservationDto Item { get; set; }

        protected Reservation Result { get; set; }

        protected IChangesHandler<IPlaceReservationDto, Reservation> ChangesHandler => Dependency<IChangesHandler<IPlaceReservationDto, Reservation>>();


        protected override void Establish_context()
        {
            base.Establish_context();
            Item = new PlaceReservationDtoTestDataBuilder()
                .Build();
        }


        protected override void Because()
        {
            Result = SUT.Create(Item);
        }

        [Test]
        public void Verify_domain_object_correctly_filled_for_creation_only()
        {
            Result.Status.ShouldBeEqualTo(ReservationStatusEnum.Requested);
        }

        [Test]
        public void Verify_changes_handler_called_for_update_fields()
        {
            ChangesHandler.AssertWasCalled(x => x.ApplyChanges(It.IsAny<Reservation>(), Item));
        }

        [Test]
        public void Verify_domain_object_not_null()
        {
            Result.ShouldNotBeNull();
        }
    }
}
