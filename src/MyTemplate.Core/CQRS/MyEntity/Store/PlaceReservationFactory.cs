namespace MyTemplate.Core.CQRS.Reservations.Place
{
    public class PlaceReservationFactory : IFactory<IPlaceReservationDto, Domain.Model.Reservation>
    {
        private readonly IChangesHandler<IPlaceReservationDto, Domain.Model.Reservation> _changesHandler;

        public PlaceReservationFactory(IChangesHandler<IPlaceReservationDto, Domain.Model.Reservation> changesHandler)
        {
            _changesHandler = changesHandler;
        }
        public Domain.Model.Reservation Create(IPlaceReservationDto model)
        {
            var entity = new ReservationBuilder()
                .WithStatus(ReservationStatusEnum.Requested) 
                .Build();

            _changesHandler.ApplyChanges(entity, model);

            return entity;
        }
    }
}