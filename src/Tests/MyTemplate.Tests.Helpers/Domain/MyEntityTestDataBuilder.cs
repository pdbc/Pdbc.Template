using Aertssen.Framework.Tests;
using MyTemplate.Domain.Model;
using MyTemplate.Domain.T4;

namespace MyTemplate.Tests.Helpers.Domain
{
    public class MyEntityTestDataBuilder : MyEntityBuilder
    {
        public MyEntityTestDataBuilder()
        {
            Name = UnitTestValueGenerator.GenerateRandomCode(16);
        }
    }
}
