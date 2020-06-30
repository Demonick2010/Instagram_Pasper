﻿using System.Collections.Generic;
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

        public string GetFullPath(string fileName)
        {
            return PathToJsonFolder + fileName;
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
}
