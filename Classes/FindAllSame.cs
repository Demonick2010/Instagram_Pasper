using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using InstagrammPasper.Models;

namespace InstagrammPasper.Classes
{
    public class FindAllSame
    {
        // Take same peoples from all lists
        public async void GetAllSomeData(TextBox resultTextBox)
        {
            List<FollowModel> resultList = new List<FollowModel>();

            await Task.Run(() =>
            {
                resultList = TakeSameTask(resultTextBox);
            });

            await Task.Run(() =>
            {
                resultList = CleanListData(resultList, resultTextBox);
            });

            await Task.Run(() =>
            {
                if (resultList != null)
                    TestDataView(resultList, resultTextBox);
                else
                    SetMessage("Result list is empty...", false, resultTextBox);
            });
        }

        private List<FollowModel> TakeSameTask(TextBox resultTextBox)
        {
            // Init current person model list
            FollowModel currentPersonFollowList = new FollowModel();
            List<FollowModel> findResultList = new List<FollowModel>();
            var followList = FollowsList.Follows;

            SetMessage("Start search method...", false, resultTextBox);

            for (int firstPerson = 0; firstPerson < followList.Count; firstPerson++)
            {
                // Get data from List to current model
                currentPersonFollowList = followList[firstPerson];

                SetMessage($"Current person: {currentPersonFollowList.PageOwnerName}", true, resultTextBox);

                for (int secondPerson = 0; secondPerson < followList.Count; secondPerson++)
                {
                    if (secondPerson == firstPerson)
                        continue;

                    // Get next person data and contain
                    var secondPersonFollowList = followList[secondPerson];
                    SetMessage($"Person for check: {secondPersonFollowList.PageOwnerName}", true, resultTextBox);

                    // First person list cycle
                    for (int firstPersonListIterator = 0; firstPersonListIterator < currentPersonFollowList.FollowsData.Count; firstPersonListIterator++)
                    {
                        // Second person list cycle
                        for (int secondPersonListIterator = 0; secondPersonListIterator < secondPersonFollowList.FollowsData.Count; secondPersonListIterator++)
                        {
                            // Check if data same, increment count
                            if (currentPersonFollowList.FollowsData[firstPersonListIterator].FollowPageAddress ==
                                secondPersonFollowList.FollowsData[secondPersonListIterator].FollowPageAddress)
                            {
                                followList[firstPerson].FollowsData[firstPersonListIterator].SameFollowCount += 1;
                                followList[firstPerson].FollowsData[firstPersonListIterator].SameFollowPeople.Add(secondPersonFollowList.PageOwnerName);

                                followList[secondPerson].FollowsData.Remove(followList[secondPerson].FollowsData[secondPersonListIterator]);
                                secondPersonListIterator--;
                            }
                        }
                    }
                }
            }
            return followList;
        }

        private List<FollowModel> CleanListData(List<FollowModel> findResultList, TextBox resultTextBox)
        {
            SetMessage("Clean wrong data from list...", false, resultTextBox);
            var sortedList = findResultList;

            // Start person cycle
            for (int i = 0; i < sortedList.Count; i++)
            {
                // Start Data List cycle
                for (int j = 0; j < sortedList[i].FollowsData.Count; j++)
                {
                    if (sortedList[i].FollowsData[j].SameFollowCount < 2)
                    {
                        sortedList[i].FollowsData.Remove(sortedList[i].FollowsData[j]);
                        j--;
                    }
                }
            }

            for (int i = 0; i < sortedList.Count; i++)
            {
                if (sortedList[i].FollowsData.Count < 1)
                {
                    sortedList.Remove(sortedList[i]);
                    i--;
                }
                    
            }
            return sortedList;
        }

        private void TestDataView(List<FollowModel> findResultList, TextBox resultTextBox)
        {
            // Test message Box
            int iterator = 1;
            string testMessage = string.Empty;
            SetMessage("Start view method...", false, resultTextBox);

            for (int i = 0; i < findResultList.Count; i++)
            {
                testMessage += $"Owner name: {findResultList[i].PageOwnerName}\n";

                for (int data = 0; data < findResultList[i].FollowsData.Count; data++)
                {
                    testMessage += "--------------------------------------------\n";

                    testMessage += $"Finded people count: {iterator} / {findResultList[i].FollowsData.Count}\n";

                    testMessage += $"Finded person name: {findResultList[i].FollowsData[data].FollowName}\n";
                    testMessage += $"Finded page same count: {findResultList[i].FollowsData[data].SameFollowCount}\n";
                    testMessage += $"Where this follows more are?: ";

                    for (int j = 0; j < findResultList[i].FollowsData[data].SameFollowPeople.Count; j++)
                    {
                        testMessage += $"@{findResultList[i].FollowsData[data].SameFollowPeople[j]}@   ";
                    }

                    testMessage += $"\n--------------------------------------------\n\n";
                    iterator++;
                }
            }

            SetMessage(testMessage, true, resultTextBox);
            findResultList.Clear();
        }

        private void SetMessage(string newMessage, bool isPlusEquals, TextBox resultTextBox)
        {
            resultTextBox.Dispatcher.Invoke(() =>
            {
                if (!isPlusEquals)
                    resultTextBox.Text = newMessage;
                else
                {
                    resultTextBox.Text += "\n" + newMessage;
                    resultTextBox.ScrollToEnd();
                }
            });
        }
    }
}

// TODO: Convert and save in XML file



