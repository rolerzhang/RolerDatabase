RolerDatabase
=============

It is a simple database manager for Win8.1 &amp; WP8.1.

How  to use:

using (SampleDatabase db = new SampleDatabase())
{
  var isExists = await db.DatabaseExists();
  if (!isExists)
  {
    await db.CreateDatabase();
    myTextBlock.Text += "DB Created" + System.Environment.NewLine;
  }

  await db.InitializeTable<Book>();   //It's necessary before use a table. 
  myTextBlock.Text += "Table [Book] Initialized" + System.Environment.NewLine;

  var books1 = db.GetTable<Book>();   //Or var books1 = db.Books;

  books1.InsertOnSubmit(new Book { ID = "1", Name = "莽荒纪", Author = "西红柿" });
  myTextBlock.Text += "Table [Book] added new data" + System.Environment.NewLine;

  await db.SubmitChanges();
  myTextBlock.Text += "DB submitted changes";
}
