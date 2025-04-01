using Microsoft.EntityFrameworkCore;
using ScpmaBe.Repositories.Entities;

namespace ScpmBe.Repositories.Infrastructure
{
    public interface IEntityBase
    {

    }

    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> GetAll();
        Task<TEntity> GetById(int id);
        Task Delete(int id);
        Task<TEntity> Update(TEntity entity);
        Task<TEntity> Insert(TEntity entity);
    }

    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntityBase
    {
        private readonly DbContext _dbContext;

        public Repository(SCPMContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public abstract Task<TEntity> GetById(int id);

        public async Task<TEntity> Insert(TEntity entity)
        {
            await Table.AddAsync(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<TEntity> Update(TEntity entity)
        {
            Table.Update(entity);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task Delete(int id)
        {
            var entity = await GetById(id);

            if (entity != null)
            {
                Table.Remove(entity);

                _dbContext.SaveChanges();
            }
        }

        protected DbSet<TEntity> Table
        {
            get
            {
                return _dbContext.Set<TEntity>();
            }
        }
    }
}