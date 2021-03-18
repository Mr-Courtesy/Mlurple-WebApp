using System.Collections.Generic;

namespace Mlurple_WebApp.Classes
{
    public static class Session
    {
        public static bool isAuthorized { get; set; } = false;
        public static List<string> projectInfo = new List<string>();
        public static string ProjectName { get; set; }
    }
}