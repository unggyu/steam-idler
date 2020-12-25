﻿using System.Text.RegularExpressions;

namespace SteamIdler.Infrastructure.Helpers
{
    public class RegexHelper
    {
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch("^[0-9]+$", value);
        }
    }
}
