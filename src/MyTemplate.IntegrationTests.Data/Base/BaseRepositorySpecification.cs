using System;
using Aertssen.Framework.Core.Model;
using Aertssen.Framework.Data.Exceptions;
using Aertssen.Framework.Data.Extensions;
using Aertssen.Framework.Data.Repositories;
using Aertssen.Framework.Tests.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyTemplate.Integration.Tests;
using NUnit.Framework;

namespace MyTemplate.IntegrationTests.Data
{
    [Parallelizable(ParallelScope.Self)]
    public abstract class BaseRepositorySpecification<TEntity> : MyTemplateIntegrationInternalBaseSpecification where TEntity : AuditableIdentifiable
    {
        public IEntityRepository<TEntity> Repository { get; set; }

        public TEntity NewItem { get; set; }
        public TEntity ExistingItem { get; set; }

        protected override void Establish_context()
        {
            base.Establish_context();

            Repository = base.ServiceProvider.GetRequiredService<IEntityRepository<TEntity>>();

            // New
            NewItem = CreateNewItem();

            // Existing
            ExistingItem = CreateExistingItem();
            Repository.Insert(ExistingItem);

            Context.SafeReload(ExistingItem);
        }

        protected abstract TEntity CreateExistingItem();

        protected abstract TEntity CreateNewItem();


        protected abstract void EditItem(TEntity entity);

        
        //protected int NumberOfObjectsOfType<T>() where T : class
        //{
        //    var repository = ServiceProvider.GetRequiredService<IEntityRepository<T>>();
        //    return repository.GetAll().Count();
        //}

        protected IEntityRepository<T> GetRepositoryFor<T>() where T : class
        {
            var repository = ServiceProvider.GetRequiredService<IEntityRepository<T>>();
            return repository;
        }

        protected override void Dispose_context()
        {
            if (NewItem != null && base.Context.Entry(NewItem).State != EntityState.Detached)
            {
                if (NewItem.Id != 0 && Repository.GetById(NewItem.Id) != null)
                {
                    // Mark as unchanged
                    base.Context.Entry(NewItem).Reload();
                    Repository.Delete(NewItem);
                }
            }

            if (ExistingItem != null && base.Context.Entry(ExistingItem).State != EntityState.Detached)
            {
                if (ExistingItem.Id != 0 && Repository.GetById(ExistingItem.Id) != null)
                {
                    // Mark as unchanged
                    base.Context.Entry(ExistingItem).Reload();
                    Repository.Delete(ExistingItem);
                }
            }
            base.Dispose_context();

        }

        [Test]
        public void Verify_object_can_be_created_by_repository()
        {
            Repository.Insert(NewItem);
            Context.SaveChanges();

            NewItem.AssertIdFilledIn();
            NewItem.AssertModifiableAuditPropertiesFilledIn();
            NewItem.AssertCreatableAuditPropertiesFilledIn();
        }

        [Test]
        public void Verify_object_can_be_deleted()
        {
            Repository.Delete(ExistingItem);
            Context.SaveChanges();

            Repository.GetById(ExistingItem.Id).ShouldBeNull();
        }

        [Test]
        public void Verify_object_can_be_updated()
        {
            EditItem(ExistingItem);
            Context.SaveChanges();

            Repository.Update(ExistingItem);
            ExistingItem.AssertModifiableAuditPropertiesFilledIn();
        }
      
        [Test]
        public void Verify_object_cannot_be_updated_when_optimistic_locking_is_wrong()
        {
            EditItem(ExistingItem);
            Context.SaveChanges();

            ExistingItem.ModifiedOn = DateTime.Now.AddSeconds(-60);

            Action action = () =>
            {
                Repository.Update(ExistingItem);
                Context.SaveChanges();
            };

            action.ShouldThrowException<DbUpdateConcurrencyException>();
        }
        
        protected void VerifyDependentObjectIsDeletedWhenDeletingEntity<TDependant>(TDependant dependentItem, TEntity entity) where TDependant : class, IIdentifiable<long>
        {
            GetRepositoryFor<TDependant>().Insert(dependentItem);
            Context.SaveChanges();

            Context.Entry(entity).Reload();
            Repository.Delete(entity);
            Context.SaveChanges();

            Repository.GetById(entity.Id).ShouldBeNull();

            Repository.GetById(entity.Id).ShouldBeNull();
            Context.SaveChanges();

            GetRepositoryFor<TDependant>().GetById(dependentItem.Id).ShouldBeNull();
        }

        protected void VerifyDependentObjectIsNotDeletedWhenDeletingEntity<TDependant>(TDependant dependentItem, TEntity entity) where TDependant : class, IIdentifiable<long>
        {
            Repository.Delete(entity);
            Context.SaveChanges();

            Repository.GetById(entity.Id).ShouldBeNull();
            Context.SaveChanges();

            GetRepositoryFor<TDependant>().GetById(dependentItem.Id).ShouldNotBeNull();
        }

        protected void VerifyEntityCannotBeDeletedWhenDependentItemIsAvailable<TDependant>(TDependant dependentItem, TEntity entity) where TDependant : class, IIdentifiable<long>
        {
            GetRepositoryFor<TDependant>().Insert(dependentItem);
            Context.SaveChanges();

            Action action = () =>
            {
                Repository.Delete(ExistingItem);
                Context.SaveChanges();
            };
            action.ShouldThrowException<DependentObjectStillUsedException>();

            GetRepositoryFor<TDependant>().Delete(dependentItem);
            Context.SaveChanges();
        }
    }
}
