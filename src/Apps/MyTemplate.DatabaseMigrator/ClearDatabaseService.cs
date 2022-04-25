using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Data;

namespace MyTemplate.DatabaseMigrator
{
    public class ClearDatabaseService
    {
        private readonly MyTemplateDbContext _dbContext;

        public ClearDatabaseService(MyTemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void ClearDatabaseTables()
        {
            // Do this a couple of times to eliminate the FK constraints issue (or make sure the tables array is correctly ordered)
            for (var i = 0; i < 3; i++)
            {
                RemoveStoredProcedures();
                RemoveTables();

                PerformAction(() => _dbContext.Database.ExecuteSqlRaw($"DROP TABLE [dbo].[__MigrationsHistory]"));
            }
        }

        private void RemoveStoredProcedures()
        {
            //PerformAction(() => _dbContext.Database.ExecuteSqlRaw($"DROP PROC Integration.SyncCompany"));
            //PerformAction(() => _dbContext.Database.ExecuteSqlRaw($"DROP PROC Integration.SyncHoliday"));
        }


        private void RemoveTables()
        {
            var tableNames = _dbContext.Model.GetEntityTypes()
                .Select(t => RelationalEntityTypeExtensions.GetTableName(t))
                .Distinct()
                .ToList();

            foreach (var table in tableNames)
            {
                PerformAction(() => _dbContext.Database.ExecuteSqlRaw($"DROP TABLE [{MyTemplateDbConstants.SchemaName.Default}].[{table}]"));
                PerformAction(() => _dbContext.Database.ExecuteSqlRaw($"DROP TABLE [{MyTemplateDbConstants.SchemaName.Integration}].[{table}]"));
            }
        }
        private static void PerformAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                // Do not log the error, only use it when debugging...
                //Log.Error(ex);
            }
        }


    }
}