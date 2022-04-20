using Aertssen.Framework.Tests;
using Moq;
using NUnit.Framework;

namespace MyTemplate.UnitTests.Core.Validators
{
    [TestFixture]
    public class PlaceReservationDtoValidatorSpecification : ContextSpecification<PlaceReservationDtoValidator>
    {
        private IDateTimeOffsetPeriodBusinessRules DateTimeOffsetPeriodBusinessRules => Dependency<IDateTimeOffsetPeriodBusinessRules>();

        protected override void Establish_context()
        {
            DateTimeOffsetPeriodBusinessRules.Stub(x => x.FromDateMustBeBeforeToDate(It.IsAny<IDateTimeOffsetPeriod>()))
                .Return(true);
        }

        [Test]
        public void Verify_date_period_validation_called()
        {
            var item = new PlaceReservationDtoTestDataBuilder().Build();
            SUT.Validate(item);
            DateTimeOffsetPeriodBusinessRules.AssertWasCalled(x=>x.FromDateMustBeBeforeToDate(item));
        }
    }
}
