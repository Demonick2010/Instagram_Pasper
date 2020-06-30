using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using InstagrammPasper.Classes;
using InstagrammPasper.Models;

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

        // Get result block in all program places
        public TextBox GeTextBox()
        {
            return ResultBlock;
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

        private async void Pasper_Click(object sender, RoutedEventArgs e)
        {
            StartPasper startPasper = new StartPasper();

            await Task.Run(() =>
            {
                startPasper.StartPasperProcess(GeTextBox());
            });
        }

        private void InitData(object sender, EventArgs e)
        {
            var messageBox = GeTextBox();
            ConstantPaths cp = new ConstantPaths();

            // JSON file paths
            const string fileName = "settings.json";
            string fullPath = cp.GetFullPath(fileName);

            const string pageLinkFileName = "pageLinks.json";
            string pathToList = cp.GetFullPath(pageLinkFileName);

            const string parsedFileName = "parsedFollowersList.json";
            string pathToParsedFile = cp.GetFullPath(parsedFileName);

            // Init Account Model

            if (File.Exists(fullPath))
            {
                AccountModel am = new AccountModel();

                UniversalSerializeDataClass<AccountModel> accountData = new UniversalSerializeDataClass<AccountModel>();
                am = accountData.DeserializeData(fullPath);

                // Check, if file exists
                if (File.Exists(pathToList))
                {
                    // Deserialize Page link list
                    UniversalSerializeDataClass<List<string>> followsData = new UniversalSerializeDataClass<List<string>>();
                    PageLinkArray.PageListLink = followsData.DeserializeData(pathToList);
                    
                    // Check for null
                    if (PageLinkArray.PageListLink == null)
                    {
                        // Set warning message to Text box
                        messageBox.Text =
                            "List link data load failed!\nPlease, add links to list!";
                        return;
                    }
                    else
                    {
                        // Set successful message to Text box
                        messageBox.Text = $"Page Link Data load successful!\nLink count: {PageLinkArray.PageListLink.Count}";
                    }
                }
                else
                {
                    // Set warning message to Text box
                    messageBox.Text += "List link data file not found!\nPlease, add links to list!";
                }

                // Check, if file exists
                if (File.Exists(pathToParsedFile))
                {
                    // Deserialize Parsed File list
                    UniversalSerializeDataClass<List<FollowModel>> folloData = new UniversalSerializeDataClass<List<FollowModel>>();
                    FollowsList.Follows = folloData.DeserializeData(pathToParsedFile);

                    // Check for null
                    if (FollowsList.Follows == null)
                    {
                        // Set warning message to Text box
                        messageBox.Text +=
                            "\nFollows people list data load failed!\nPlease, start to parse!";
                        return;
                    }
                    else
                    {
                        // Set successful message to Text box
                        messageBox.Text += $"\nFollows people list data load successful!\nPeople count: {FollowsList.Follows.Count}";
                    }
                }
                else
                {
                    // Set warning message to Text box
                    messageBox.Text += "\nFollows people list data file not found!\nPlease, start to parse!";
                }

                // Set values to static model
                if (am != null)
                {
                    TempUserData.UserName = am.UserName;
                    TempUserData.UserPassword = am.UserPassword;
                }

                // Set successful message to Text box
                messageBox.Text += "\nUser Data load successful!";
            }
            else
            {
                // Set warning message to Text box
                messageBox.Text += "\nPlease, set user data to Settings!";
            }
        }

        private async void ShowResult_Click(object sender, RoutedEventArgs e)
        {
            FindAllSame findAllSame = new FindAllSame();

            ShowResult.IsEnabled = false;

            await Task.Run(() =>
            {
                findAllSame.GetAllSomeData(GeTextBox());
            });

            ShowResult.IsEnabled = true;
        }
    }
}
