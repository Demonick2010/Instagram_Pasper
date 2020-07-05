using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace InstagrammPasper.Classes
{
    public static class AppMessages
    {
        public static void SetMessage(string newMessage, bool isPlusEquals, TextBox resultTextBox)
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

        public static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^\w\s\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return string.Empty;
            }
        }
    }
}
