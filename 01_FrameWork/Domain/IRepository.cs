using System.Linq.Expressions;

namespace _0_FrameWork.Domain
{
    public interface IRepository<Tkey, T> where T : class 
    {
        T Get(Tkey id);
        List<T> Get();
        void Create(T entity);
        bool Exists(Expression<Func<T, bool>> expression);
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
