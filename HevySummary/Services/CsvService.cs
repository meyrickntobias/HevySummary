using System.Text;
using HevySummary.Models;

namespace HevySummary.Services;

public class CsvService
{
    /*
     * Hevy provides all of your workouts in CSV format
     * The workouts are provided as rows of sets so there will be N rows if there were N sets in the exercise
     * The exercises are given as titles without the template IDs so that will have to be looked up
     * 
     */
    public List<Workout> ImportHevyCsv(string[] csvLines)
    {
        List<Workout> workouts = [];
        
        // Skip header line
        foreach (var csvLine in csvLines.Skip(1))
        {
            var cols = ParseLineIntoColumns(csvLine);
        }
        
        return workouts;
    }
    
    // Parse line into separate columns
    public List<string> ParseLineIntoColumns(string line)
    {
        var tokens = new List<string>();
        var currentToken = new StringBuilder();
        var isWithinQuotes = false;
        
        for (var i = 0; i < line.Length; i++)
        {
            var currentChar = line[i];
            
            isWithinQuotes = currentChar switch
            {
                '"' when !isWithinQuotes => true,
                '"' when isWithinQuotes => false,
                _ => isWithinQuotes
            };
    
            if (i == line.Length - 1)
            {
                tokens.Add(currentToken.ToString());
            }

            if (currentChar != ',' || isWithinQuotes)
            {
                currentToken.Append(currentChar);
                continue;
            };
            
            tokens.Add(currentToken.ToString());
            currentToken.Clear();
            isWithinQuotes = false;
        }
        
        return tokens;
    }

}