using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using InstagrammPasper.Classes;
using InstagrammPasper.Models;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace InstagrammPasper
{
    /// <summary>
    /// Interaction logic for General.xaml
    /// </summary>
    public partial class General : Window
    {
        public General()
        {
            InitializeComponent();
        }
        private void Window_Left_Mouse_Down(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }

        private void Pasper_Click(object sender, RoutedEventArgs e)
        {
            StartPasper sp = new StartPasper();
            sp.StartPasperProcess();
        }

        private void InitData(object sender, EventArgs e)
        {
            // JSON file path
            string _path = $"{Directory.GetCurrentDirectory()}\\Setting\\";
            string _fileName = "settings.json";
            string _fullPath = _path + _fileName;

            string _pageLinkFileName = "pageLinks.json";
            string pathToList = _path + _pageLinkFileName;

            // Init Account Model
            AccountModel _am = new AccountModel();

            if (File.Exists(_fullPath))
            {
                var serializer = new JsonSerializer();

                // Deserialize name and pass
                using var sw = new StreamReader(_fullPath);
                using (var reader = new JsonTextReader(sw))
                {
                    _am = serializer.Deserialize<AccountModel>(reader);
                    reader.Close();
                }
                sw.Close();

                // Check, if file exists
                if (File.Exists(pathToList))
                {
                    // Deserialize Page link list
                    using var sw2 = new StreamReader(pathToList);
                    using (var reader = new JsonTextReader(sw2))
                    {
                        PageLinkArray.PageListLink = serializer.Deserialize<List<string>>(reader);
                        reader.Close();
                    }

                    sw2.Close();

                    // Check for null
                    if (PageLinkArray.PageListLink == null)
                    {
                        // Set warning message to Text box
                        ResultBlock.Foreground = new SolidColorBrush(Colors.Red);
                        ResultBlock.Text = "List link data load failed!\nPlease, add links to list!";
                        return;
                    }
                    else
                    {
                        // Set successful message to Text box
                        ResultBlock.Foreground = new SolidColorBrush(Colors.Green);
                        ResultBlock.Text +=
                            $"\nPage Link Data load successful!\nLink count: {PageLinkArray.PageListLink.Count}";
                    }
                }
                else
                {
                    // Set warning message to Text box
                    ResultBlock.Foreground = new SolidColorBrush(Colors.Red);
                    ResultBlock.Text += "List link data file not found!\nPlease, add links to list!";
                }

                // Set values to static model
                TempUserData.UserName = _am.UserName;
                TempUserData.UserPassword = _am.UserPassword;

                // Set successful message to Text box
                ResultBlock.Foreground = new SolidColorBrush(Colors.Green);
                ResultBlock.Text += "\nUser Data load successful!";
            }
            else
            {
                // Set warning message to Text box
                ResultBlock.Foreground = new SolidColorBrush(Colors.Red);
                ResultBlock.Text += "\nPlease, set user data to Settings!";
            }
        }
    }
}
