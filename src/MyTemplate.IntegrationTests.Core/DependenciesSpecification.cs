using System;
using Aertssen.Framework.Tests.Extensions;
using MyTemplate.Integration.Tests;
using NUnit.Framework;

namespace MyTemplate.IntegrationTests.Core
{
    public class DependenciesSpecification : MyTemplateIntegrationInternalBaseSpecification
    {
        //[TestCase(typeof(IAuditRecordLoggerService))]
        public void VerifyDependencyCanBeResolved(Type type)
        {
            var resolved = ServiceProvider.GetService(type);
            resolved.ShouldNotBeNull();
        }
    }
}