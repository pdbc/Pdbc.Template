namespace MyTemplate.Core.CQRS.Reservations.Place
{
    public class PlaceReservationCommandValidator : FluentValidationValidator<PlaceReservationCommand>
    {
        public PlaceReservationCommandValidator(IValidator<IPlaceReservationDto> placeReservationValidator)
        {
            RuleFor(x => x.Reservation)
                .SetValidator(placeReservationValidator);

        }
    }
}