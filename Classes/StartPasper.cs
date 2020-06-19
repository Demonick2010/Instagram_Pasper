using System;
using System.Threading;
using System.Threading.Tasks;
using InstagrammPasper.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Extensions;

namespace InstagrammPasper.Classes
{
    class StartPasper
    {
        public void StartPasperProcess()
        {
            LoginPasperTask();
        }

        private void LoginPasperTask()
        {
            // Browser options
            var mozillaOptions = new FirefoxOptions();
            mozillaOptions.PageLoadStrategy = PageLoadStrategy.Normal;

            // Init web driver
            IWebDriver driver = new FirefoxDriver(mozillaOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);

            // Go to user page
            driver.Navigate().GoToUrl(StartAdress.adress);

            /* LOGIN PAGE */

            // Find element by name
            var formElement = driver.FindElement(By.TagName("form"));
            var usernameElement = formElement.FindElement(By.Name("username"));
            var passwordElement = formElement.FindElement(By.Name("password"));
            var loginButton = formElement.FindElement(By.TagName("button"));

            // Set username
            usernameElement.SendKeys(TempUserData.UserName);
            passwordElement.SendKeys(TempUserData.UserPassword);

            // Wait 4 seconds
            Thread.Sleep(4000);

            // Click login button
            loginButton.Click();

            // Wait 4 seconds and check login success
            while (true)
            {
                // Wait 4 seconds
                Thread.Sleep(4000);

                // Get current URL
                string checkUrl = driver.Url;

                // Check urls
                if (StartAdress.adress == checkUrl)
                {
                    // Click login button
                    loginButton.Click();
                }
                else
                {
                    break;
                }
            }


            /**************/

            /* Find element to applay save */

            // Wait 4 seconds and click the button
            Thread.Sleep(4000);

            // Find section element and button
            var saveFormElement = driver.FindElement(By.TagName("section"));
            var saveButton = saveFormElement.FindElement(By.TagName("button"));

            saveButton.Click();



            /*******************************/

            /*Go to parsing page and parse*/

            for (int i = 0; i < PageLinkArray.PageListLink.Count; i++)
            {
                // Go to parsing page
                driver.Navigate().GoToUrl(PageLinkArray.PageListLink[i]);

                // Find parent container in page
                var headerBlock = driver.FindElement(By.TagName("header"));

                // Find current following link
                var allLink = headerBlock.FindElements(By.TagName("li"));
                
                // Wait 4 seconds
                Thread.Sleep(4000);

                // Check for null and click
                allLink[2].Click();

                // Scroll to down
                var modalElement = driver.FindElement(By.XPath("//body/div[@role='presentation']/div[@role='dialog']//ul"));

                //driver.ExecuteJavaScript(
                //    "var timeId = setInterval( function() {if(window.scrollY<(document.body.scrollHeight-window.screen.availHeight))window.scrollTo(0,document.body.scrollHeight);else{clearInterval(timeId);window.scrollTo(0,0);}},500);"
                //    );

                //Get scroll height
                var lastHeight = driver.ExecuteJavaScript<long>("return arguments[0].scrollHeight", modalElement);
                int tryTimes = 0;
                int scrollDelay = 3000;
                int listPosition = 12;

                while (true)
                {


                    var tempModal = driver.FindElement(By.XPath("//body/div[@role='presentation']/div[@role='dialog']//ul"));

                    // TODO: Try Catch construction
                    var tempModalDialog = driver.FindElement(By.XPath($"//body/div[@role='presentation']/div[@role='dialog']//ul/div/li[{listPosition}]"));

                    //Scroll down to bottom
                    //driver.ExecuteJavaScript("arguments[0].scroll(0,800)", tempModal);
                    driver.ExecuteJavaScript("arguments[0].scrollIntoView(true);", tempModalDialog);

                    //Wait to load page
                    Thread.Sleep(scrollDelay);

                    //body/div[@role='presentation']/div[@role='dialog']//ul

                    //Calculate new scroll height and compare with last scroll height
                    var newHeight = driver.ExecuteJavaScript<long>("return arguments[0].scrollHeight", tempModal);

                    if (lastHeight == newHeight)
                        tryTimes += 1;

                    if (tryTimes > 3)
                    {
                        tryTimes = 0;
                        break;
                    }

                    lastHeight = newHeight;
                    listPosition += 12;
                }

                // TODO: Parse peoples to model and to list
                Thread.Sleep(15000);

                // TODO: Take same peoples from all lists

                // TODO: Convert and save in XML file
            }

            /**/
        }
    }
}
