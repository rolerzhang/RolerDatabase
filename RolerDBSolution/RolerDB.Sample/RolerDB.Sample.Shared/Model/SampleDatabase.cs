using RolerFramework.Database;

namespace RolerDB.Sample.Model
{
    public class SampleDatabase : RolerDatabase
    {
        private const string _DBAddress = "RolerDB";

        public Table<Book> Books;

        public SampleDatabase()
            : base(_DBAddress)
        {
        }
    }
}
