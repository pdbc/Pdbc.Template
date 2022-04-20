using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MyTemplate.Data
{
    public class MyTemplateDbContextFactory : IDesignTimeDbContextFactory<MyTemplateDbContext>
    {
        public MyTemplateDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyTemplateDbContext>();
            optionsBuilder.UseSqlServer("data source=.\\;Database=Template;integrated security=True;MultipleActiveResultSets=True;");

            return new MyTemplateDbContext(optionsBuilder.Options);
        }
    }
}