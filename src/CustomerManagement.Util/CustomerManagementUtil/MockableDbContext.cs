using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data;

namespace CustomerManagement.Util
{
    public abstract class MockableDbContext : DbContext, IMockableDBContext
    {
        public MockableDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        { }

        #region IMockableDBContext implementation

        int IMockableDBContext.SaveChanges()
        {
            return this.SaveChanges();
        }

        public virtual T Save<T>(T entity) where T : class
        {
            return this.GetSet<T>().Add(entity);
        }

        public virtual T Update<T>(T entity) where T : class
        {
            entity = this.GetSet<T>().Attach(entity);
            this.Entry<T>(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual T Delete<T>(T entity) where T : class
        {
            return this.GetSet<T>().Remove(entity);
        }

        public virtual T Delete<T>(long id) where T : class
        {
            return Delete<T>(this.GetSet<T>().Find(id));
        }

        public virtual T Attach<T>(T entity) where T : class
        {
            return this.GetSet<T>().Attach(entity);
        }
        #endregion

        #region DbContext extensions

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        #endregion

        #region Testability methods

        // We need to mock this so we can verify that objects get added to the set upon Save()
        protected virtual IDbSet<T> GetSet<T>() where T : class
        {
            return this.Set<T>();
        }

        #endregion
    }
}
