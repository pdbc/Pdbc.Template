using Aertssen.Framework.Tests;
using NUnit.Framework;

namespace MyTemplate.UnitTests.Core.CQRS.Reservations.Place
{
    [TestFixture]
    public class PlaceReservationChangesHandlerSpecification : ContextSpecification<PlaceReservationChangesHandler>
    {
        protected PlaceReservationDto Item { get; set; }

        protected Reservation DomainObject { get; set; }

        protected override void Establish_context()
        {
            base.Establish_context();

            Item = new PlaceReservationDtoTestDataBuilder().Build();
            DomainObject = new ReservationTestDataBuilder().Build();
        }

        protected override void Because()
        {
            SUT.ApplyChanges(DomainObject, Item);
        }

        [Test]
        public void Verify_domain_object_correctly_filled()
        {
            DomainObject.FromDate.ShouldBeEqualTo(Item.FromDate);
            DomainObject.ToDate.ShouldBeEqualTo(Item.ToDate);

        }
    }
}