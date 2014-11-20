using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace RolerFramework.Database
{
    [System.Runtime.Serialization.DataContract]
    public sealed class Table<TEntity> : ITable, ITable<TEntity>, IEnumerable, IEnumerable<TEntity>
        where TEntity : class
    {
        #region Fields

        [System.Runtime.Serialization.DataMember(Name = "Entities")]
        private List<TEntity> _entities;

        #endregion

        #region Properties

        #region Interface: ITable

        public bool IsInitialied
        {
            get { return this._entities != null; }
        }

        #endregion

        #endregion

        #region Constructor

        internal Table()
        {
        }

        #endregion

        #region Methods

        #region Interface: ITable

        void ITable.InsertOnSubmit(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var tEntity = entity as TEntity;
            if (tEntity == null)
            {
                throw new InvalidOperationException("The entity is not of the correct type.");
            }

            this.InsertOnSubmit(tEntity);
        }

        void ITable.InsertAllOnSubmit(IEnumerable entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.CheckInitialied();

            var list = entities.Cast<object>().ToList();
            ITable table = this;
            foreach (object entity in list)
            {
                table.InsertOnSubmit(entity);
            }
        }

        void ITable.DeleteOnSubmit(object entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var tEntity = entity as TEntity;
            if (tEntity == null)
            {
                throw new InvalidOperationException("The entity is not of the correct type.");
            }

            this.DeleteOnSubmit(tEntity);
        }

        void ITable.DeleteAllOnSubmit(IEnumerable entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.CheckInitialied();

            var list = entities.Cast<object>().ToList();
            ITable table = this;
            foreach (object entity in list)
            {
                table.DeleteOnSubmit(entity);
            }
        }

        #endregion

        #region Interface: ITable<TEntity>

        public void InsertOnSubmit(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            this.CheckInitialied();

            this._entities.Add(entity);
        }

        public void DeleteOnSubmit(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            this.CheckInitialied();

            this._entities.Remove(entity);
        }

        #endregion

        #region Interface: IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region Interface: IEnumerable<TEntity>

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        public IEnumerator<TEntity> GetEnumerator()
        {
            this.CheckInitialied();

            return this._entities.GetEnumerator();
        }

        public void InsertAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : TEntity
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.CheckInitialied();

            foreach (TEntity entity in entities)
            {
                this.InsertOnSubmit(entity);
            }
        }

        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities) where TSubEntity : TEntity
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }
            this.CheckInitialied();

            foreach (TEntity entity in entities)
            {
                this.DeleteOnSubmit(entity);
            }
        }

        private void CheckInitialied()
        {
            if (!this.IsInitialied)
            {
                throw new InvalidOperationException("Table is not initialized");
            }
        }

        public override string ToString()
        {
            return "Table(" + typeof(TEntity).Name + ")";
        }

        #endregion
    }
}
