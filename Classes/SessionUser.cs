using System;
using System.Collections.Generic;

namespace Mlurple_WebApp.Classes
{
    public static class SessionUser
    {
        public static string username { get; set; }
        public static List<string> projects = new List<string>();
    }
}