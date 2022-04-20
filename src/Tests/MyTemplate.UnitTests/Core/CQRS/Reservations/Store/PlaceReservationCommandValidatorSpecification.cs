using Aertssen.Framework.Tests;
using Aertssen.Framework.Tests.Infra.Extensions;
using FluentValidation;
using NUnit.Framework;

namespace MyTemplate.UnitTests.Core.CQRS.Reservations.Place
{
    [TestFixture]
    public class PlaceReservationCommandValidatorSpecification : ContextSpecification<PlaceReservationCommandValidator>
    {
        protected PlaceReservationCommand Command { get; set; }

        protected FluentValidation.Results.ValidationResult ValidationResult { get; set; }

        protected IValidator<IPlaceReservationDto> ScharnierStoreDtoValidator => Dependency<IValidator<IPlaceReservationDto>>();

        protected override void Establish_context()
        {
            base.Establish_context();
            Command = new PlaceReservationCommandTestDataBuilder()
                .Build();
        }

        protected override void Because()
        {
            ValidationResult = SUT.Validate(Command);
        }

        [Test]
        public void Verify_store_Scharnier_validator_called()
        {
            ScharnierStoreDtoValidator.ExpectSubValidatorCalled();
        }
    }
}