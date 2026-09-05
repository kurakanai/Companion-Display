using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CompanionDisplayWinUI.API.PerformanceHelpers
{
    partial class RegexHelper
    {
        [GeneratedRegex(@"^\[(\d{2}:\d{2}\.\d{2})\](.*)?")]
        public static partial Regex TimestampRegex();
        [GeneratedRegex(@"[^\w\s]")]
        public static partial Regex StripSpecialCharsRegex();
    }
}
