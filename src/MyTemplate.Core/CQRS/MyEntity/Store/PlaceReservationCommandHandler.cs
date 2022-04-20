using System.Threading;
using System.Threading.Tasks;

namespace MyTemplate.Core.CQRS.Reservations.Place
{
    public class PlaceReservationCommandHandler : IRequestHandler<PlaceReservationCommand, Nothing>
    {
        private readonly IFactory<IPlaceReservationDto, Reservation> _factory;
        private readonly IReservationRepository _repository;

        public PlaceReservationCommandHandler(IFactory<IPlaceReservationDto, Reservation> factory,
            IReservationRepository repository)
        {
            _factory = factory;
            _repository = repository;
        }

        public Task<Nothing> Handle(PlaceReservationCommand request, CancellationToken cancellationToken)
        {
            Reservation entity;
            
            entity = _factory.Create(request.Reservation);
            _repository.Insert(entity);
           

            var nothing = Nothing.AtAll();
            return Task.FromResult(nothing);
        }
    }
}