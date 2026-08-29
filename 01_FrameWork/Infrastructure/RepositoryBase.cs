using _0_FrameWork.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace _0_FrameWork.Infrastructure
{
    public class RepositoryBase<Tkey, T> : IRepository<Tkey, T> where T : EntityBase
    {
        private readonly DbContext _context;

        public RepositoryBase(DbContext context)
        {
            _context = context;
        }

        public void Create(T entity)
        {
            _context.Add(entity);
        }

        public bool Exists(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(x => !x.IsDeleted)
                .Any(expression);
        }

        public T Get(Tkey id)
        {
            return _context.Set<T>().FirstOrDefault(x => x.Id.Equals(id) && !x.IsDeleted);
        }

        public List<T> Get()
        {
            return _context.Set<T>().Where(x => !x.IsDeleted)
                .ToList();  
        }

        public T GetIncludingDeleted(Tkey id)
        {
            return _context.Set<T>()
                .FirstOrDefault(x => x.Id.Equals(id));
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
