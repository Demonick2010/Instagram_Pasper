using System;
using System.Collections.Generic;
using System.Text;

namespace InstagrammPasper.Models
{
    public class AccountModel
    {
        public string UserName { get; set; }
        public string UserPassword { get; set; }
    }

    public static class TempUserData
    {
        public static string UserName { get; set; }
        public static string UserPassword { get; set; }
    }

    public static class StartAdress
    {
        public static string adress = "https://www.instagram.com/";
    }

    public static class PageLinkArray
    {
        public static List<string> PageListLink { get; set; } = new List<string>();
    }
}
