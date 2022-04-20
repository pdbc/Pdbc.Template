using Aertssen.Framework.Data.Repositories;
using MyTemplate.Domain.Model;

namespace MyTemplate.Data.Repositories
{
    public interface IMyEntityRepository : IEntityRepository<MyEntity>
    {

    }

    public class MyEntityRepository : EntityFrameworkRepository<MyEntity>, IMyEntityRepository
    {
        public MyEntityRepository(MyTemplateDbContext dbContext) : base(dbContext)
        {

        }
    }
}
