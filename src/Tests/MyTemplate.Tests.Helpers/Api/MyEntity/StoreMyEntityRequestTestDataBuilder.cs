using Pdbc.Demo.Api.Contracts.Requests.Reservations;

namespace MyTemplate.Tests.Helpers.Api.MyEntity
{
    public class StoreMyEntityRequestTestDataBuilder : PlaceReservationRequestBuilder
    {
        public StoreMyEntityRequestTestDataBuilder()
        {
            Reservation = new PlaceReservationDtoTestDataBuilder();
        }
    }
}
