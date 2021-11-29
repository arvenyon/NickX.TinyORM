using NickX.TinyORM.Persistence.Queries;
using System.Collections.Generic;

namespace NickX.TinyORM.Persistence.Repositories.Interfaces
{
    public interface IRepository<T> where T : class, new()
    {
        //void BulkInsert(T[] entity);
        //void BulkUpdate(T[] entity);
        //void BulkDelete(T[] entity);
        object Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        bool Exists(QueryConditionBuilder<T> queryConditionBuilder);
        T Single(object key);
        T Single(QueryConditionBuilder<T> queryConditionBuilder);
        IEnumerable<T> All();
        IEnumerable<T> Multiple(QueryConditionBuilder<T> queryConditionBuilder);
    }
}
