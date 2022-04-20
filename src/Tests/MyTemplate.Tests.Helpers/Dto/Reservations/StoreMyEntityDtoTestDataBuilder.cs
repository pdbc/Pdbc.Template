using System;
using Aertssen.Framework.Tests;
using MyTemplate.Core.T4;
using MyTemplate.Dto.MyEntity;

namespace MyTemplate.Tests.Helpers.Dto.Reservations
{
    public class StoreMyEntityDtoTestDataBuilder : StoreMyEntityDtoBuilder
    {
        public StoreMyEntityDtoTestDataBuilder()
        {
            Name = UnitTestValueGenerator.GenerateRandomCode();
        }
    }
}
