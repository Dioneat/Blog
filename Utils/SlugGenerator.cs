using System.Text.RegularExpressions;

namespace Blog10.Utils
{
    public static class SlugGenerator
    {
        private static readonly Dictionary<string, string> CyrillicToLatinMap = new()
        {
            {"а", "a"}, {"б", "b"}, {"в", "v"}, {"г", "g"}, {"д", "d"},
            {"е", "e"}, {"ё", "yo"}, {"ж", "zh"}, {"з", "z"}, {"и", "i"},
            {"й", "y"}, {"к", "k"}, {"л", "l"}, {"м", "m"}, {"н", "n"},
            {"о", "o"}, {"п", "p"}, {"р", "r"}, {"с", "s"}, {"т", "t"},
            {"у", "u"}, {"ф", "f"}, {"х", "h"}, {"ц", "ts"}, {"ч", "ch"},
            {"ш", "sh"}, {"щ", "sch"}, {"ъ", ""}, {"ы", "y"}, {"ь", ""},
            {"э", "e"}, {"ю", "yu"}, {"я", "ya"}
        };

        public static string Generate(string phrase)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return "";

            string str = phrase.ToLower();

            foreach (var pair in CyrillicToLatinMap)
                str = str.Replace(pair.Key, pair.Value);

            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = str.Replace(" ", "-");

            return str;
        }
    }
}
