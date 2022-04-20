using System;

namespace MyTemplate.Dto.MyEntity
{
    public interface IMyEntityDataDto 
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class MyEntityDataDto : IMyEntityDataDto
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

}
