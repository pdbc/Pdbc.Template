using System;
using Aertssen.Framework.Api.Contracts;
using Aertssen.Framework.Tests.Infra;
using MyTemplate.Data;
using Microsoft.EntityFrameworkCore;

namespace MyTemplate.Integration.Tests.IntegrationTests
{
    public abstract class BaseMyTemplateServiceTest<TResult> : BaseIntegrationTest<TResult> where TResult : AertssenResponse
    {
        public MyTemplateDbContext DbContext { get; }
        
        protected TestCaseService TestCaseService { get; set; }
        protected MyTemplateTestDataObjects MyTemplateTestDataObjects { get; set; }

        protected BaseMyTemplateServiceTest(MyTemplateDbContext dbContext)
        {
            DbContext = dbContext;

            TestCaseService = new TestCaseService(dbContext);
            MyTemplateTestDataObjects = new MyTemplateTestDataObjects(dbContext);

            MyTemplateTestDataObjects.LoadObjects();
            MyTemplateTestDataObjects.SetupMissingObjects();
        }

        protected override void SaveChanges()
        {
            DbContext.SaveChanges();
        }

        protected override Boolean IsLocalTest()
        {
            //var configuration = Container.Resolve<IConfiguration>();
            //var connectionString = IdentityStoreDbContextConnectionStringProvider.GetConnectionString(configuration);
            //return connectionString.Contains("localhost");

            var connectionString = DbContext.Database.GetConnectionString();
            return connectionString.Contains("localhost");
        }
    }
}