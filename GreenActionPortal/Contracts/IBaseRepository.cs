using System.Linq.Expressions;

namespace GreenActionPortal.Contracts
{
    public enum ErrorCode
    {
        Success,
        Error
    }

    public interface IBaseRepository<T>
    {
        T Get(object id);

        List<T> GetAll();
        ErrorCode Create(T t);
        ErrorCode Update(object id, T t);
        ErrorCode Delete(object id);

        IEnumerable<T> FindByCondition(Expression<Func<T, bool>> expression);
    }
}
