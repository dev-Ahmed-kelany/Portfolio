using System.Text.RegularExpressions;

namespace Portfolio.Business
{
    public static class clsUtility
    {
        /// <summary>
        /// Validates if a string matches a given regex pattern
        /// </summary>
        /// <param name="input">The input string to validate</param>
        /// <param name="pattern">The regex pattern to match against</param>
        /// <returns>True if input matches the pattern, false otherwise</returns>
        public static bool MatchRegex(string input, string pattern)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern))
                return false;

            try
            {
                return Regex.IsMatch(input, pattern);
            }
            catch
            {
                return false;
            }
        }
    }
}
