using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RolerFramework.Database
{
    public interface ITable<TEntity> where TEntity : class
    {

        void InsertOnSubmit(TEntity entity);
        void DeleteOnSubmit(TEntity entity);

    }
}
