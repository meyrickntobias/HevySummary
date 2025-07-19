using System.Text;

namespace HevySummary.Helpers;

public static class StringExtensions
{
    public static string CapitalizeFirstLetterOfEachWord(this string input)
    {
        var splitWords = input.Split("_");
        var wordArray = new string[splitWords.Length];
        for (var i = 0; i < splitWords.Length; i++)
        {
            var word = splitWords[i];
            var sb = new StringBuilder();
            sb.Append(char.ToUpperInvariant(word[0]));
            if (word.Length == 1)
            {
                wordArray[i] = sb.ToString();
                continue;
            }
            sb.Append(word[1..]);
            wordArray[i] = sb.ToString();
        }

        return string.Join(' ', wordArray); ;
    }
}