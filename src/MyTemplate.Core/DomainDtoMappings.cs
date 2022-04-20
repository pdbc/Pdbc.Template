using AutoMapper;
using MyTemplate.Domain.Model;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Core
{
    public class DomainDtoMappings : Profile
    {
        public DomainDtoMappings()
        {
            SetupReservationsMappings();
        }

        public void SetupReservationsMappings()
        {
            CreateMap<MyEntity, MyEntityDataDto>();
        }
    }
}