using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using InstagrammPasper.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Extensions;
using static InstagrammPasper.Classes.AppMessages;

namespace InstagrammPasper.Classes
{
    public class StartPasper
    {
        public async void StartPasperProcess(TextBox resulTextBox)
        {
            // Browser options
            var mozillaOptions = new FirefoxOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal
            };

            // Init web driver
            IWebDriver driver = new FirefoxDriver(mozillaOptions);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);

            await Task.Run(() =>
            {
                LoginPasperTask(driver);
                SetMessage("Login Success! Go to find follows.", false, resulTextBox);
            });

            if (DoneActionList.LoginIsDone)
            {
                await Task.Run(() =>
                {
                    StartParse(driver, resulTextBox);
                    SetMessage("Parse success! Next step - Save to JSON.", true, resulTextBox); 
                });
            }

            if (DoneActionList.StartParseIsDone)
            {
                await Task.Run(() =>
                {
                    SaveDataToJson(driver, resulTextBox);
                });
            }
                
        }

        private void LoginPasperTask(IWebDriver driver)
        {
            // Go to user page
            ConstantPaths paths = new ConstantPaths();
            driver.Navigate().GoToUrl(paths.StartAddress);

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
                if (paths.StartAddress == checkUrl)
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

            DoneActionList.LoginIsDone = true;
        }

        private void StartParse(IWebDriver driver, TextBox resultTextBox)
        {
            // Clear static array
            FollowsList.Follows.Clear();

            /*Go to parsing page and parse*/

            for (int i = 0; i < PageLinkArray.PageListLink.Count; i++)
            {
                // Go to parsing page
                driver.Navigate().GoToUrl(PageLinkArray.PageListLink[i]);

                // Add message
                SetMessage($"Open page {PageLinkArray.PageListLink[i]} successful", false, resultTextBox);

                // Find parent container in page
                var headerBlock = driver.FindElement(By.TagName("header"));

                // Find current following link
                var allLink = headerBlock.FindElements(By.TagName("li"));
                var followsCount = Convert.ToInt32(allLink[2].FindElement(By.TagName("span")).Text.Replace(" ", ""));
                var pageName = headerBlock.FindElement(By.XPath("//div[@id='react-root']/section/main[@role='main']//section//h2")).Text;
                
                // Wait 4 seconds
                Thread.Sleep(4000);

                // Check for null and click
                allLink[2].Click();

                SetMessage($"Open the follows window success", true, resultTextBox);

                // Scroll to down
                var modalElement = driver.FindElement(By.XPath("//body/div[@role='presentation']/div[@role='dialog']//ul"));

                // Get scroll height
                var lastHeight = driver.ExecuteJavaScript<long>("return arguments[0].scrollHeight", modalElement);

                // Needed vars
                int tryTimes = 0;
                int scrollDelay = 1500;
                int listPosition = 12;

                while (true)
                {
                    try
                    {
                        var tempModalDialog = driver.FindElement(By.XPath(
                            $"//body/div[@role='presentation']/div[@role='dialog']//ul/div/li[{listPosition}]"));

                        //Scroll down to bottom
                        driver.ExecuteJavaScript("arguments[0].scrollIntoView(true);", tempModalDialog);

                        //Wait to load page
                        Thread.Sleep(scrollDelay);

                        //Calculate new scroll height and compare with last scroll height
                        var newHeight =
                            driver.ExecuteJavaScript<long>("return arguments[0].scrollHeight", modalElement);

                        if (lastHeight == newHeight)
                            tryTimes += 1;

                        if (tryTimes > 3)
                        {
                            SetMessage($"Scrolling complete.", false, resultTextBox);
                            tryTimes = 0;
                            break;
                        }

                        lastHeight = newHeight;

                        listPosition += 12;

                        // Check position count
                        if (listPosition > followsCount)
                            listPosition = followsCount;

                        SetMessage($"Next Scroll follow position is: {listPosition}", true, resultTextBox);
                    }
                    catch (Exception e)
                    {
                        var followsLinksObjCount =
                            driver.FindElements(By.XPath(
                                "/html/body/div[@role='presentation']/div[@role='dialog']//ul/div/li/div/div/div[2]//div/a"));

                        SetMessage(e.Message, false, resultTextBox);
                        SetMessage($"Current element count: {followsLinksObjCount.Count}\nGo to last element!", true, resultTextBox);

                        listPosition = followsLinksObjCount.Count;
                        tryTimes = 4;
                    }
                }

                // Parse peoples to model and to list
                SetMessage($"Parse the Links data from page, Please, wait ...", true, resultTextBox);

                var followsLinksObj =
                    driver.FindElements(By.XPath("/html/body/div[@role='presentation']/div[@role='dialog']//ul/div/li/div/div/div[2]//div/a"));

                SetMessage($"Links parse to object complete. Links count: {followsLinksObj.Count}", true, resultTextBox);

                SetMessage($"Parse the Names data from page, Please, wait ...", true, resultTextBox);

                var followsNameObj =
                    driver.FindElements(By.XPath("/html/body/div[@role='presentation']/div[@role='dialog']//ul/div/li/div/div/div[2]//div[2]"));

                FollowModel flm = new FollowModel
                {
                    PageOwnerName = pageName
                };

                SetMessage($"Parsing complete!", true, resultTextBox);

                SetMessage($"Start adding data to model, Please Wait ...", true, resultTextBox);

                for (int j = 0; j < followsLinksObj.Count; j++)
                {
                    string link = "https://www.instagram.com" + "/" + followsLinksObj[j].Text + "/";
                    string name = followsNameObj[j].Text;

                    if (string.IsNullOrWhiteSpace(name))
                        name = followsLinksObj[j].Text;

                    // Add data to model
                    flm.FollowsData.Add(new FollowData
                    {
                        FollowName = name,
                        FollowPageAddress = link,
                        SameFollowCount = 0
                    });
                }

                FollowsList.Follows.Add(flm);
                SetMessage($"Adding complete", true, resultTextBox);
            }

            DoneActionList.StartParseIsDone = true;

            /***************************************/
        }

        // Save result to JSON
        private void SaveDataToJson(IWebDriver driver, TextBox resultTextBox)
        {
            ConstantPaths paths = new ConstantPaths();

            SetMessage($"Start to serialize data to JSON file ...", true, resultTextBox);

            // Create paths
            string jsonFileName = "parsedFollowersList.json";
            string fullPath = paths.GetFullPath(jsonFileName);

            // Check, if directory not exists, create directory
            if (!Directory.Exists(paths.PathToJsonFolder))
                Directory.CreateDirectory(paths.PathToJsonFolder);

            // Create JSON file if not exists
            if (!File.Exists(fullPath))
            {
                var file = File.Create(fullPath);
                file.Close();
            }
            else
            {
                File.Delete(fullPath);

                var file = File.Create(fullPath);
                file.Close();
            }

            
            UniversalSerializeDataClass<List<FollowModel>> serializeData = new UniversalSerializeDataClass<List<FollowModel>>();
            serializeData.SerializeData(FollowsList.Follows, fullPath);
            
            driver.Quit();

            SetMessage($"All data saved!\nWell done!\nPeople count: {FollowsList.Follows.Count}\nNow you can convert to all same addresses to XML", true, resultTextBox);
        }
    }
}
