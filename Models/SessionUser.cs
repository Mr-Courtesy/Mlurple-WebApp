using System;
using System.Collections.Generic;

namespace Mlurple_WebApp.Models
{
    public static class SessionUser
    {
        public static string username { get; set; }
        public static List<string> projects = new List<string>();
    }
}