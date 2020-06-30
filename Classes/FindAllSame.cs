using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using InstagrammPasper.Models;

namespace InstagrammPasper.Classes
{
    public class FindAllSame
    {
        // TODO: Take same peoples from all lists
        public async void GetAllSomeData(TextBox resultTextBox)
        {
            await Task.Run(() =>
            {
                TakeSameTask(resultTextBox);
            });
        }

        private void TakeSameTask(TextBox resultTextBox)
        {
            // Init current person model list
            FollowModel currentPersonFollowList = new FollowModel();
            List<FollowModel> findResultList = new List<FollowModel>();

            for (int firstPerson = 0; firstPerson < FollowsList.Follows.Count; firstPerson++)
            {
                // Get data from List to current model
                currentPersonFollowList = FollowsList.Follows[firstPerson];
                SetMessage($"Current person: {currentPersonFollowList.PageOwnerName}", true, resultTextBox);

                int secondPerson = firstPerson + 1;

                if (secondPerson < FollowsList.Follows.Count)
                {
                    // Get next person data and contain
                    var secondPersonFollowList = FollowsList.Follows[secondPerson];
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
                                SetMessage($"Find! Current ID: {currentPersonFollowList.FollowsData[firstPersonListIterator]}\n Second person list ID: {secondPersonFollowList.FollowsData[secondPersonListIterator]}", true, resultTextBox);

                                // Change value in current model
                                currentPersonFollowList.FollowsData[firstPersonListIterator].SameFollowCount += 1;
                                currentPersonFollowList.FollowsData[firstPersonListIterator].SameFollowPeople.Add(secondPersonFollowList.PageOwnerName);

                                // Change value in general array (v0.1)
                                //FollowsList.Follows[firstPerson].FollowsData[firstPersonListIterator].SameFollowCount += 1;
                                //FollowsList.Follows[firstPerson].FollowsData[firstPersonListIterator].SameFollowPeople.Add(secondPersonFollowList.FollowsData[secondPersonListIterator].FollowName);

                                // Add data to filtered array
                                findResultList.Add(currentPersonFollowList);
                            }
                        }
                    }
                }
            }

            // Test message Box
            int iterator = 1;
            string testMessage = string.Empty;
            foreach (var item in findResultList)
            {
                foreach (var follows in item.FollowsData)
                {
                    if (follows.SameFollowCount > 1)
                    {
                        testMessage += "--------------------------------------------\n";

                        testMessage += $"Finded people count: {iterator} / {findResultList.Count}\n";
                        testMessage += $"Owner name: {item.PageOwnerName}\n";

                        testMessage += $"Finded person name: {follows.FollowName}\n";
                        testMessage += $"Finded page same count: {follows.SameFollowCount}\n";
                        testMessage += $"Where this follows more are?: ";

                        foreach (var samePeopleFollow in follows.SameFollowPeople)
                        {
                            testMessage += $"@{samePeopleFollow}@ , ";
                        }

                        testMessage += $"\n--------------------------------------------\n\n";
                    }
                }
                
            }

            //if (string.IsNullOrWhiteSpace(testMessage))
            //    testMessage = "Same content not found! Sorry..";

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

        // TODO: Convert and save in XML file
    }
}
