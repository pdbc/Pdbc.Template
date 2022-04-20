using Aertssen.Framework.Tests;
using AutoMapper;
using NUnit.Framework;

namespace MyTemplate.UnitTests
{
    public class RequestMappingsTestFixture : BaseSpecification
    {
        [Test]
        public void Test_RequestToCqrsMappings_is_valid()
        {
            var configuration = new MapperConfiguration(cfg => {
                //cfg.AddProfile<RequestToCqrsMappings>();
                //cfg.AddProfile<CqrsServiceRequestToCqrsMappings>();
            });

            configuration.AssertConfigurationIsValid();
        }
    }
}
