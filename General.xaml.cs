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
            // Browser options
            var mozillaOptions = new FirefoxOptions();
            mozillaOptions.PageLoadStrategy = PageLoadStrategy.Normal;

            // Init web driver
            IWebDriver driver = new FirefoxDriver(mozillaOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Go to user page
            driver.Navigate().GoToUrl(StartAdress.adress);

            // Find element by name
            var formElement = driver.FindElement(By.TagName("form"));
            var usernameElement = formElement.FindElement(By.Name("username"));
            var passwordElement = formElement.FindElement(By.Name("password"));
            var loginButton = formElement.FindElement(By.TagName("button"));

            // Set username
            usernameElement.SendKeys(TempUserData.UserName);
            passwordElement.SendKeys(TempUserData.UserPassword);
            loginButton.Click();

            // 
        }

        private void InitData(object sender, EventArgs e)
        {
            // JSON file path
            string _path = $"{Directory.GetCurrentDirectory()}\\Setting\\";
            string _fileName = "settings.json";
            string _fullPath = _path + _fileName;

            // Init Account Model
            AccountModel _am = new AccountModel();

            if (File.Exists(_fullPath))
            {
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
                    ResultBlock.Foreground = new SolidColorBrush(Colors.Red);
                    ResultBlock.Text = "Data load failed!";
                    return;
                }

                // Set values to static model
                TempUserData.UserName = _am.UserName;
                TempUserData.UserPassword = _am.UserPassword;
            }
        }
    }
}
