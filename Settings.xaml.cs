using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        ConstantPaths cp;
        string _fileName = "settings.json";
        string _fullPath;
        string _pageLinkFileName = "pageLinks.json";

        AccountModel _am;

        public Settings()
        {
            InitializeComponent();
            cp = new ConstantPaths();
            _fullPath = cp.GetFullPath(_fileName);
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
                if (!Directory.Exists(cp.PathToJsonFolder))
                    Directory.CreateDirectory(cp.PathToJsonFolder);

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
                ErrorLabel.Content = "\nSave data success!";

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
            string pathToFile = cp.GetFullPath(_pageLinkFileName);

            // Check Address List existing
            if (File.Exists(pathToFile))
            {
                var serializer = new JsonSerializer(); 

                // Deserialize name and pass
                using var sr = new StreamReader(pathToFile);
                using (var reader = new JsonTextReader(sr))
                {
                    PageLinkArray.PageListLink = serializer.Deserialize<List<string>>(reader);
                    reader.Close();
                }
                sr.Close();

                // Check null
                if (PageLinkArray.PageListLink != null)
                {
                    // Change color and set successful message
                    ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                    ErrorLabel.Content += $"Page link list load success!\nList count: {PageLinkArray.PageListLink.Count}";

                    // Add dresses to text block
                    foreach (var item in PageLinkArray.PageListLink)
                    {
                        PageLinkBlock.Text += item;
                        PageLinkBlock.Text += "\r\n";
                    }
                }
                else
                {
                    // Change color and set failed message
                    ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
                    ErrorLabel.Content += "Page link list load FAILED!\nPlease, add links to list!";
                }
            }
            else
            {
                // Change color and set failed message
                ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
                ErrorLabel.Content += "Page link list NOT EXISTS!\nPlease, add links to list!";
            }

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
                    ErrorLabel.Content += "\nAccount Data load failed!";
                    File.Delete(_fullPath);
                    return;
                }

                // Set values to textboxes
                UserNameBox.Text = _am.UserName;
                PasswordBox.Password = _am.UserPassword;

                // Change color and set successful message
                ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                ErrorLabel.Content += "\nAccount Data load success!";
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

        private void SaveListButton_Click(object sender, RoutedEventArgs e)
        {
            if (PageLinkBlock != null)
            {
                string pathToFile = cp.GetFullPath(_pageLinkFileName);
                List<string> listDeserialize;

                string tempString = PageLinkBlock.Text;
                var tempArrayLinks = tempString.Replace("\r\n", ",").Split(',').ToList();

                // Create JSON file if not exists
                if (!File.Exists(pathToFile))
                {
                    var file = File.Create(pathToFile);
                    file.Close();
                }

                var serializer = new JsonSerializer();

                // Deserialize name and pass
                using var sr = new StreamReader(pathToFile);
                using (var reader = new JsonTextReader(sr))
                {
                    listDeserialize = serializer.Deserialize<List<string>>(reader);
                    reader.Close();
                }
                sr.Close();

                // Check deserialize list for null
                if (listDeserialize != null)
                {
                    // Clear List
                    listDeserialize.Clear();

                    // Add all data to list
                    listDeserialize.AddRange(tempArrayLinks);

                    // Add values to static list
                    PageLinkArray.PageListLink.Clear();
                    PageLinkArray.PageListLink.AddRange(listDeserialize);
                }
                else
                {
                    listDeserialize = new List<string>();
                    listDeserialize.AddRange(tempArrayLinks);

                    // Add values to static list
                    PageLinkArray.PageListLink = new List<string>();
                    PageLinkArray.PageListLink.AddRange(listDeserialize);
                }

                // Check for empty items in array
                for (int i = 0; i < PageLinkArray.PageListLink.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(PageLinkArray.PageListLink[i]))
                        PageLinkArray.PageListLink.Remove(PageLinkArray.PageListLink[i]);
                }

                // Save values to JSON file
                using var sw = new StreamWriter(pathToFile);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, PageLinkArray.PageListLink);
                    writer.Close();
                }
                sw.Close();

                ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                ErrorLabel.Content = "\nPage address link data Saved!";
            }
            else
            {
                // Path to file
                string pathToFile = cp.GetFullPath(_pageLinkFileName);
                List<string> listDeserialize;

                // Create JSON file if not exists
                if (!File.Exists(pathToFile))
                {
                    var file = File.Create(pathToFile);
                    file.Close();
                }

                var serializer = new JsonSerializer();

                // Deserialize name and pass
                using var sr = new StreamReader(pathToFile);
                using (var reader = new JsonTextReader(sr))
                {
                    listDeserialize = serializer.Deserialize<List<string>>(reader);
                    reader.Close();
                }
                sr.Close();

                if (listDeserialize == null)
                {
                    ErrorLabel.Foreground = new SolidColorBrush(Colors.Red);
                    ErrorLabel.Content += "\nPage adress link is empty!\nPlease, add addresses to list";
                }
                else
                {
                    ErrorLabel.Foreground = new SolidColorBrush(Colors.Green);
                    ErrorLabel.Content += "\nPage adress link data loaded!";
                }
            }
        }
    }
}
