using System.Collections.Generic;

namespace InstagrammPasper.Models
{
    public class FollowModel
    {
        public string PageOwnerName { get; set; }
        public List<FollowData> FollowsData { get; set; } = new List<FollowData>();
    }

    public class FollowData
    {
        public string FollowName { get; set; }
        public string FollowPageAddress { get; set; }
        public int SameFollowCount { get; set; }
        public List<string> SameFollowPeople { get; set; } = new List<string>();
    }

    public static class FollowsList
    {
        public static List<FollowModel> Follows { get; set; } = new List<FollowModel>();
    }

    public class FollowDataToSorting
    {
        public string FollowName { get; set; }
        public string FollowPageAddress { get; set; }
        public int SameFollowCount { get; set; }
        public string SameFollowPeople { get; set; }
    }
}
