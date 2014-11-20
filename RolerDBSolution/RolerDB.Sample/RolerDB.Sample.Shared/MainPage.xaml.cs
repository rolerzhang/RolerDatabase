using RolerDB.Sample.Model;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace RolerDB.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
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
        }
    }
}
