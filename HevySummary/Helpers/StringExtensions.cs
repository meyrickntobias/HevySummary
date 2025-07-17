using System.Text;

namespace HevySummary.Helpers;

public static class StringExtensions
{
    public static string CapitalizeFirstLetterOfEachWord(this string input)
    {
        var splitWords = input.Split(" ");
        var sb = new StringBuilder();
        foreach (var word in splitWords)
        {
            sb.Append(char.ToUpperInvariant(word[0]));
            if (word.Length == 1)
            {
                continue;
            }
            sb.Append(word[1..]);
        }
        return sb.ToString();
    }
}