using System;
using Aertssen.Framework.Tests.Infra;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MyTemplate.IntegrationTests.Api
{
    public abstract class MyTemplateIntegrationApiODataRequestSpecification : MyTemplateIntegrationBaseApiRequestSpecification
    {
        protected IIntegrationTest SkipIntegrationTest;
        protected abstract IIntegrationTest CreateSkipIntegrationTest();

        protected IIntegrationTest TopIntegrationTest;
        protected abstract IIntegrationTest CreateTopIntegrationTest();

        protected IIntegrationTest FilterIntegrationTest;
        protected abstract IIntegrationTest CreateFilterIntegrationTest();

        protected IIntegrationTest OrderByIntegrationTest;
        protected abstract IIntegrationTest CreateOrderByIntegrationTest();

        [Test]
        public void Execute_OData_Skip_Test()
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Running {TestExecutionContext.CurrentContext.CurrentTest.FullName}");

            SkipIntegrationTest = CreateSkipIntegrationTest();
            ExecuteTest(SkipIntegrationTest);

            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Finished {TestExecutionContext.CurrentContext.CurrentTest.FullName}");
        }

        [Test]
        public void Execute_OData_Top_Test()
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Running {TestExecutionContext.CurrentContext.CurrentTest.FullName}");

            TopIntegrationTest = CreateTopIntegrationTest();
            ExecuteTest(TopIntegrationTest);

            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Finished {TestExecutionContext.CurrentContext.CurrentTest.FullName}");
        }

        [Test]
        public void Execute_OData_Filter_Test()
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Running {TestExecutionContext.CurrentContext.CurrentTest.FullName}");

            FilterIntegrationTest = CreateFilterIntegrationTest();
            ExecuteTest(FilterIntegrationTest);

            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Finished {TestExecutionContext.CurrentContext.CurrentTest.FullName}");
        }

        [Test]
        public void Execute_OData_OrderBy_Test()
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Running {TestExecutionContext.CurrentContext.CurrentTest.FullName}");

            OrderByIntegrationTest = CreateOrderByIntegrationTest();
            ExecuteTest(OrderByIntegrationTest);

            TestExecutionContext.CurrentContext.OutWriter.WriteLine($"{DateTime.Now:hh:mm:ss.fffffff}: Finished {TestExecutionContext.CurrentContext.CurrentTest.FullName}");
        }

        private void ExecuteTest(IIntegrationTest test)
        {
            EditApiTest();

            using (var transaction = Context.Database.BeginTransaction())
            {
                test.Setup();
                Context.SaveChanges();

                transaction.Commit();
            }


            test.Run();
            test.Cleanup();

            // Save changes after cleanup
            Context.SaveChanges();
        }
    }
}
