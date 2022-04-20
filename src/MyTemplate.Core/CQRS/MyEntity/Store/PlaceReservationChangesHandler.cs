namespace MyTemplate.Core.CQRS.Reservations.Place
{
    public class PlaceReservationChangesHandler : IChangesHandler<IPlaceReservationDto, Domain.Model.Reservation>
    {
        public void ApplyChanges(Domain.Model.Reservation entity, IPlaceReservationDto model)
        {
            // Apply the changes
            entity.FromDate = model.FromDate;
            entity.ToDate = model.ToDate;
        }
    }
}