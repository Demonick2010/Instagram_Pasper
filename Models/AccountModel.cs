using System.Collections.Generic;
using System.IO;

namespace InstagrammPasper.Models
{
    public class AccountModel
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }

    public class ConstantPaths
    {
        public string StartAddress = "https://www.instagram.com/";
        public string PathToJsonFolder = $"{Directory.GetCurrentDirectory()}\\Setting\\";
        public string PathToExcelFolder = $"{Directory.GetCurrentDirectory()}\\Result\\";

        public string[] ColumNames = new string[]
        {
            "Name of global subscription found",
            "Link to the page",
            "The number of matches in people",
            "Who coincided"
        };

        public string GetFullPath(string fileName)
        {
            return PathToJsonFolder + fileName;
        }

        public string GetFullPathToResult(string fileName)
        {
            // Get full path
            return PathToExcelFolder + fileName;
        }
    }

    public static class TempUserData
    {
        public static string UserName { get; set; }
        public static string UserPassword { get; set; }
    }

    public static class PageLinkArray
    {
        public static List<string> PageListLink { get; set; } = new List<string>();
    }
    public static class AboutTextClass
    {
        public static string AboutTextBlock { get; set; } = "How to use the program?\n1. Open settings and write account data and parsing links list.\n2. Save the data and start parse the data (Start Parse Button)\n3. Press 'Show Data' button, and programm delete from parsed file all unnecessary data (followers < 2) and save new data to JSON file.\n4. Press 'Save to Excel' button, and program convert all data to Excel table and give the link.\n\nCreated by DEMONICK. All Rights Reserved!\nEmail: masterprogger87@gmail.com\n";
    }
}
