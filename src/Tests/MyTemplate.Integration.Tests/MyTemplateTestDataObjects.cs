using System.Linq;
using MyTemplate.Data;
using MyTemplate.Domain.Model;

namespace MyTemplate.Integration.Tests
{
    public class MyTemplateTestDataObjects
    {
        public MyTemplateDbContext DbContext { get; set; }
        private TestCaseService TestCaseService { get; set; }

        public MyTemplateTestDataObjects(MyTemplateDbContext dbContext)
        {
            DbContext = dbContext;
            TestCaseService = new TestCaseService(DbContext);
        }

        public void LoadObjects()
        {
            
        }

        public void SetupMissingObjects()
        {
          
        }
    }
}