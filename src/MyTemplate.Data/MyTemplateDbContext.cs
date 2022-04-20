using System;
using System.ComponentModel.DataAnnotations;
using Aertssen.Framework.Data.Extensions;
using Aertssen.Framework.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyTemplate.Data.Configurations;
using MyTemplate.Domain.Model;

namespace MyTemplate.Data
{
    public partial class MyTemplateDbContext : DbContext
    {
        private readonly ILogger<MyTemplateDbContext> _logger;

        public MyTemplateDbContext(DbContextOptions<MyTemplateDbContext> options) : base(options)
        {
        }

        public MyTemplateDbContext(
            DbContextOptions<MyTemplateDbContext> options,
            ILogger<MyTemplateDbContext> logger) : base(options)
        {
            _logger = logger;
        }

        #region DbSets
        public virtual DbSet<MyEntity> MyEntities { get; set; }
       
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO put this behind configuration
            //optionsBuilder.LogTo(Console.WriteLine)
            //    .EnableSensitiveDataLogging()
            //    .EnableDetailedErrors();

            optionsBuilder.AddInterceptors(new DatabaseCommandInterceptor());

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");
            modelBuilder.HasDefaultSchema(DemoDbConstants.SchemaName.Default);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyEntityConfiguration).Assembly);
            //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuditRecordConfiguration).Assembly);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


        public override int SaveChanges()
        {
            // Extension method from FW to validate the entities (
            this.ValidateEntities();

            this.HandleCreatableEntities(GetExecutingUserName);
            this.HandleModifiableEntities(GetExecutingUserName);
            //this.ApplyDescriptionsForAuditRecord();

            // AuditRecords (lifecycle)
            //var auditRecordCreationInfoItems = this.GetAuditRecordsCreationInfoItems();

            try
            {
                var numberOfChanges = base.SaveChanges();

                //SaveAuditRecords(auditRecordCreationInfoItems);

                return numberOfChanges;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                invalidOperationException.ThrowErrorWhenDependentObjectStillUsedException();
                throw;
            }
            catch (ValidationException dbEntityValidationException)
            {
                _logger.LogError($"TokenDbContext.SaveChanges failed: Validation failed: {dbEntityValidationException.ValidationResult}");
                throw;
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                _logger.LogError($"Optimistic locking failed", dbUpdateConcurrencyException);
                throw;
            }
            catch (DbUpdateException dbUpdateException)
            {
                dbUpdateException.ThrowErrorWhenDependentObjectStillUsedException();
                dbUpdateException.ThrowErrorWhenUniqueIndexViolated();

                throw;
            }
        }

        #region Helper methods

        private string GetExecutingUserName()
        {
            //return UserContext.GetUsername();
            return "USERNAME_TODO";
        }

        #endregion

        #region Auditing - Full

        //private void SaveAuditRecords(List<AuditRecordCreationDataInfo> records)
        //{
        //    if (_auditLoggerService == null)
        //        return;


        //    records.ForEach(x =>
        //    {
        //        //_auditPropertiesResolver.LoadAuditProperties(x.Entity);

        //        var auditProperties = x.Entity.GetAuditProperties();
        //        if (auditProperties == null)
        //        {
        //            throw new ArgumentException("AuditPropertiesCannotBeResolved");
        //        }

        //        _auditLoggerService.LogObjectLifecycleChange(auditProperties, (EntityActionEnum)x.Action, x.PropertyChanges)
        //            .GetAwaiter()
        //            .GetResult();

        //    });

        //}

        #endregion
    }
}