using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FilePickerMultiContextTest
{
    public sealed partial class MainPage : Page
    {
        private User User { get; set; }

        public MainPage()
        {
            InitializeComponent();
            TestPickerButton.Click += TestPickerButton_Click;
        }

        private async void TestPickerButton_Click(object sender, RoutedEventArgs e)
        {
            if (User == null)
            {
                StatusText.Text = "Invalid user";
                return; // Do not launch the File Picker, the user was not picked.
            }

            // Initializing a FileOpenPicker as we would do normally.
            FileOpenPicker fileOpenPicker = FileOpenPicker.CreateForUser(User);
            fileOpenPicker.FileTypeFilter.Add("*");
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            fileOpenPicker.ViewMode = PickerViewMode.List;

            /* WORKAROUND
             * Getting a folder for a user is initializing the FileOpenPicker window handle.
             * We must get a folder that obviously exists so the app doesn't crash
             * Even if the folder doesn't exist, the FileOpenPicker still works after using this function.
             * Make sure to still use a try/catch to avoid any failure
             */
            try
            {
                await StorageFolder.GetFolderFromPathForUserAsync(User, ApplicationData.Current.LocalFolder.Path);
            }
            catch { }

            // Open the FileOpenPicker, that now works, no exception is thrown; and it returns a valid file.
            StorageFile pickedFile = await fileOpenPicker.PickSingleFileAsync();
            if (pickedFile == null)
            {
                StatusText.Text = string.Empty;
            }
            else
            {
                StatusText.Text = pickedFile.Path;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is User)
            {
                User = (User)e.Parameter; // Get the user from the App OnLaunched argument e.User.
            }
        }
    }
}
