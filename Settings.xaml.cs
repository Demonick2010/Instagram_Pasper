using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using InstagrammPasper.Models;
using Newtonsoft.Json;


namespace InstagrammPasper
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        // JSON file path
        string _path = $"{Directory.GetCurrentDirectory()}\\Setting\\";
        string _fileName = "settings.json";
        string _fullPath;
        AccountModel _am;

        public Settings()
        {
            InitializeComponent();
            _fullPath = _path + _fileName;
            _am = new AccountModel();
        }

        private void Window_Left_Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Check null values
            if (string.IsNullOrWhiteSpace(UserNameBox.Text) && string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                ErrorLabel.Content = "User name of password cannot be empty!";
                return;
            }

            // Set values to model
            _am.UserName = UserNameBox.Text;
            _am.UserPassword = PasswordBox.Password;

            // Set data to static model
            TempUserData.UserName = _am.UserName;
            TempUserData.UserPassword = _am.UserPassword;

            if (IsSaved.IsChecked ?? false)
            {
                // Check, if directory not exists, create directory
                if (!Directory.Exists(_path))
                    Directory.CreateDirectory(_path);

                // Create JSON file if not exists
                if (!File.Exists(_fullPath))
                {
                    var file = File.Create(_fullPath);
                    file.Close();
                }

                var serializer = new JsonSerializer();

                using (var sw = new StreamWriter(_fullPath))
                {
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, _am);
                        writer.Close();
                    }
                    sw.Close();
                }

                // Print success message
                ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                ErrorLabel.Content = "Save data success!";

                // Set save button to disable
                SaveButton.IsEnabled = false;
            }
            else
            {
                TempUserData.UserName = _am.UserName;
                TempUserData.UserPassword = _am.UserPassword;

                // Print success message
                ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                ErrorLabel.Content = "Data temporary saved! \nRemember set new data next time!";
            }
        }

        private void InitAccountData(object sender, RoutedEventArgs e)
        {
            if (File.Exists(_fullPath))
            {
                // Save button disable
                SaveButton.IsEnabled = false;

                var serializer = new JsonSerializer(); 

                // Deserialize name and pass
                using (var sw = new StreamReader(_fullPath))
                {
                    using (var reader = new JsonTextReader(sw))
                    {
                        _am = serializer.Deserialize<AccountModel>(reader);
                        reader.Close();
                    }
                    sw.Close();
                }

                // Check for null
                if (_am == null)
                {
                    ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
                    ErrorLabel.Content = "Data load failed!";
                    File.Delete(_fullPath);
                    return;
                }

                // Set values to textboxes
                UserNameBox.Text = _am.UserName;
                PasswordBox.Password = _am.UserPassword;

                // Change color and set successful message
                ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                ErrorLabel.Content = "Data load success!";
            }
            else
            {
                // Set save button to enable
                SaveButton.IsEnabled = true;
            }
        }

        private void UserNameChanged(object sender, RoutedEventArgs e)
        {
            if (!SaveButton.IsEnabled)
                SaveButton.IsEnabled = !SaveButton.IsEnabled;
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!SaveButton.IsEnabled)
                SaveButton.IsEnabled = !SaveButton.IsEnabled;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
