using System.Collections;

namespace RolerFramework.Database
{
    public interface ITable
    {
        bool IsInitialied { get; }

        void InsertOnSubmit(object entity);
        void InsertAllOnSubmit(IEnumerable entities);
        void DeleteOnSubmit(object entity);
        void DeleteAllOnSubmit(IEnumerable entities);
    }
}
