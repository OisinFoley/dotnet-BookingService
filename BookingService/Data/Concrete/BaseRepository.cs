using BookingService.Data.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Data.Concrete
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected IUnitOfWork UnitOfWork { get; set; }

        protected ApplicationContext Context => (ApplicationContext)UnitOfWork;

        protected BaseRepository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected virtual DbSet<TEntity> GetDbSet()
        {
            return Context.Set<TEntity>();
        }

        protected virtual void SetEntityState(object entity, EntityState entityState)
        {
            Context.Entry(entity).State = entityState;
        }

        public async Task<TEntity> FindAsync(params object[] keyValues)
        {
            try
            {
                return await GetDbSet().FindAsync(keyValues);
            }
            catch (Exception e)
            {
                Trace.Write(e.Message);
                throw;
            }
        }

        public virtual IQueryable<TEntity> SelectQuery(string query, params object[] parameters)
        {
            return GetDbSet().FromSql(query, parameters).AsQueryable();
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity)
        {
            TEntity retval;

            try
            {
                retval = GetDbSet().Add(entity).Entity;
                await SaveContextAsync();
            }
            catch (Exception e)
            {
                Trace.Write(e.Message);
                throw;
            }

            return retval;
        }

        public virtual async Task<int> UpdateAsync(TEntity entity)
        {
            try
            {
                GetDbSet().Attach(entity);
                SetEntityState(entity, EntityState.Modified);

                return await SaveContextAsync();
            }
            catch (Exception e)
            {
                Trace.Write(e.Message);
                throw;
            }
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            GetDbSet().Attach(entity);
            SetEntityState(entity, EntityState.Deleted);
            return await SaveContextAsync();
        }

        protected async Task<int> SaveContextAsync()
        {
            try
            {
                return await UnitOfWork.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                foreach (EntityEntry entry in ex.Entries)
                {
                    if (entry.Entity is TEntity)
                    {
                        // Using a NoTracking query means we get the entity but it is not tracked by the context
                        // and will not be merged with existing entities in the context.
                        object[] entityKey = GetEntityKey(entry.Entity);
                        TEntity databaseEntity = await FindAsync(entityKey);
                        EntityEntry<TEntity> databaseEntry = Context.Entry(databaseEntity);

                        foreach (IProperty property in entry.Metadata.GetProperties())
                        {
                            object databaseValue = databaseEntry.Property(property.Name).CurrentValue;

                            // Selecting StoreWins strategy for now
                            entry.Property(property.Name).CurrentValue = databaseValue;

                            // Update original values to 
                            entry.Property(property.Name).OriginalValue = databaseEntry.Property(property.Name).CurrentValue;
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("Don't know how to handle concurrency conflicts for " + entry.Metadata.Name);
                    }
                }

                // Retry the save operation
                await UnitOfWork.SaveChangesAsync();
            }

            return -1;
        }

        private object[] GetEntityKey<T>(T entity) where T : class
        {
            EntityEntry<T> state = Context.Entry(entity);
            IEntityType metadata = state.Metadata;
            IKey key = metadata.FindPrimaryKey();
            IProperty[] props = key.Properties.ToArray();

            return props.Select(x => x.GetGetter().GetClrValue(entity)).ToArray();
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
        }
    }
}
