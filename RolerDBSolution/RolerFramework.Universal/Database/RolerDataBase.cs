using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Windows.Storage;
using System.Collections.Generic;

namespace RolerFramework.Database
{
    public class RolerDatabase : IDisposable
    {
        #region Fields

        private string _dbName;
        private StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private Dictionary<string, ITable> _tables;

        #endregion

        #region Constructor

        public RolerDatabase(string dbName)
        {
            this._dbName = dbName;

            this._tables = new Dictionary<string, ITable>();
            this.InitTables();
        }

        #endregion

        #region Methods

        public Table<TEntity> GetTable<TEntity>()
            where TEntity : class
        {
            var tableType = typeof(TEntity);
            if (!this.IsInTables(tableType))
            {
                throw new InvalidOperationException("type is not marked as table");
            }

            ITable table = this.GetTable(tableType);

            return (Table<TEntity>)table;
        }

        public async Task InitializeTable<TEntity>()
            where TEntity : class
        {
            var table = this.GetTable<TEntity>();
            if (!table.IsInitialied)
            {
                var tableTypeInfo = table.GetType().GetTypeInfo();
                var field = tableTypeInfo.DeclaredFields.FirstOrDefault(p => p.Name == "_entities");
                if (field != null)
                {
                    var desTable = await this.ReadTableFromFile<TEntity>();
                    if (desTable == null)
                    {
                        field.SetValue(table, new List<TEntity>());
                    }
                    else
                    {
                        var eneities = field.GetValue(desTable);
                        field.SetValue(table, eneities);
                    }
                }
            }
        }

        private void InitTables()
        {
            var fields = this.GetType().GetTypeInfo().DeclaredFields.Where(p => p.IsPublic);
            foreach (FieldInfo fieldInfo in fields)
            {
                var fieldTypeInfo = fieldInfo.FieldType.GetTypeInfo();
                if (fieldTypeInfo.IsGenericType && fieldTypeInfo.GetGenericTypeDefinition() == typeof(Table<>))
                {
                    ITable table = (ITable)fieldInfo.GetValue(this);
                    if (table == null)
                    {
                        Type elementType = fieldTypeInfo.GenericTypeArguments[0];
                        table = this.GetTable(elementType);
                        fieldInfo.SetValue(this, table);
                    }
                }
            }

        }

        private ITable GetTable(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            ITable result;

            var tableKey = this.GetTableKey(type);

            if (!this._tables.TryGetValue(tableKey, out result))
            {
                Type tableType = typeof(Table<>).MakeGenericType(type);
                var tbConstructor = tableType.GetTypeInfo().DeclaredConstructors.FirstOrDefault();
                if (tbConstructor == null)
                {
                    throw new System.InvalidOperationException("no internal constructor");
                }
                result = (ITable)tbConstructor.Invoke(new object[] { });
                this._tables.Add(tableKey, result);
            }

            return result;
        }

        private bool IsInTables(Type type)
        {
            var tableKey = this.GetTableKey(type);
            return this._tables.ContainsKey(tableKey);
        }

        #region Database

        public async Task CreateDatabase()
        {
            await this.localFolder.CreateFolderAsync(this._dbName);
        }

        public async Task ResetDatabase()
        {
            await this.localFolder.CreateFolderAsync(this._dbName, CreationCollisionOption.ReplaceExisting);
        }

        public async Task<bool> DatabaseExists()
        {
            StorageFolder folder = null;
            try
            {
                folder = await this.localFolder.GetFolderAsync(this._dbName);
            }
            catch
            {

            }
            return folder != null;
        }

        public async Task DeleteDatabase()
        {
            StorageFolder folder = null;
            try
            {
                folder = await this.localFolder.GetFolderAsync(this._dbName);
                await folder.DeleteAsync();
            }
            catch
            {

            }
        }

        #endregion

        /// <summary>
        /// 提交更改
        /// </summary>
        /// <returns></returns>
        public virtual async Task SubmitChanges()
        {
            foreach (var tableKey in this._tables.Keys)
            {
                if (this._tables[tableKey].IsInitialied)
                {
                    await SaveTableToFile(tableKey);
                }
            }
        }

        private string GetTableKey(Type tableType)
        {
            if (tableType == null)
            {
                throw new ArgumentNullException("tableType");
            }

            return tableType.ToString();
        }

        /// <summary>
        /// 动态创建一个类型的对象
        /// </summary>
        /// <param name="t"></param>
        /// <param name="paramas"></param>
        /// <returns></returns>
        private object CreateInstance(Type t, object[] paramas)
        {
            int pCount = paramas == null ? 0 : paramas.Length;
            foreach (var item in t.GetTypeInfo().DeclaredConstructors)
            {
                var p = item.GetParameters();
                if (p.Length == pCount)
                    return item.Invoke(paramas);
            }
            throw new InvalidOperationException("没有找到合适的构造函数");
        }

        #region Table I/O

        /// <summary>
        /// 根据文件名，将文件内容反序列化成某个对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task<Table<TEntity>> ReadTableFromFile<TEntity>()
            where TEntity : class
        {
            StorageFolder storageFolder = await this.localFolder.CreateFolderAsync(this._dbName, CreationCollisionOption.OpenIfExists);
            StorageFile file = null;
            Table<TEntity> result;
            try
            {
                var fileName = this.GetTableKey(typeof(TEntity));
                file = await storageFolder.GetFileAsync(fileName);
            }
            catch (Exception)
            {

            }
            if (file == null)
            {
                result = null;
            }
            else
            {
                string jsonString = await FileIO.ReadTextAsync(file);
                result = SerializationHelper.JsonDeserialize<Table<TEntity>>(jsonString);
            }
            return result;
        }

        /// <summary>
        /// 将某个对象序列化成文件，如果传递的对象为null，那么删除原来的文件
        /// </summary>
        /// <param name="tableKey"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private async Task SaveTableToFile(string tableKey)
        {
            if (!this._tables.ContainsKey(tableKey))
            {
                throw new KeyNotFoundException("key not found!");
            }

            StorageFolder storageFolder = await this.localFolder.CreateFolderAsync(this._dbName, CreationCollisionOption.OpenIfExists);
            var storageFile = await storageFolder.CreateFileAsync(tableKey, CreationCollisionOption.ReplaceExisting);

            var table = this._tables[tableKey];
            string jsonString = SerializationHelper.JsonSerializer(table);
            await FileIO.WriteTextAsync(storageFile, jsonString);
        }

        #endregion

        #endregion

        public void Dispose()
        {
        }
    }
}
