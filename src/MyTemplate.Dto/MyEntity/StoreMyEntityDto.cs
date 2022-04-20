using System;

namespace MyTemplate.Dto.MyEntity
{
    public interface IStoreMyEntityDto
    {
        String Name { get; set; }
    }
    public class StoreMyEntityDto : IStoreMyEntityDto
    {
        public string Name { get; set; }
    }
}