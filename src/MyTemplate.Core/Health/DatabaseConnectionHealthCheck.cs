using System.Linq;
using Aertssen.Framework.Core.Health;
using MyTemplate.Data;

namespace MyTemplate.Core.Health
{
    public class DatabaseConnectionHealthCheck : HealthTest
    {
        private readonly MyTemplateDbContext _context;

        public DatabaseConnectionHealthCheck(MyTemplateDbContext context)
        {
            _context = context;
        }

        public override void Execute(HealthTestContext info)
        {
            var numberOfReservations = _context.MyEntities.Count();
        }
        
    }
}
