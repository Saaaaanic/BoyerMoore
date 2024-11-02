namespace BoyerMoore;

public class BoyerMoore : ISubstringSearch
{
    private readonly int[] _badCharTable;
    private readonly string _pattern;
    private readonly int _patternLength;

    public BoyerMoore(string pattern)
    {
        _pattern = pattern;
        _patternLength = pattern.Length;
        _badCharTable = BuildBadCharacterTable(pattern);
    }

    // Public method to search for pattern in text
    public List<int> Search(string text, string pattern, out long stepCount)
    {
        if (pattern != _pattern)
        {
            throw new ArgumentException("Pattern does not match the initialized pattern.");
        }
        
        stepCount = 0;
        List<int> occurrences = new List<int>();
        int textLength = text.Length;
        int i = 0; // index in text

        while (i <= textLength - _patternLength)
        {
            int j = _patternLength - 1;

            // Compare pattern with text from end to start
            while (j >= 0 && _pattern[j] == text[i + j])
            {
                stepCount++; // Step: Character match
                j--;
            }

            if (j < 0)
            {
                // Match found at index i
                occurrences.Add(i);
                // Shift pattern using good suffix heuristic
                if (i + _patternLength < textLength)
                {
                    i += _patternLength - _badCharTable[text[i + _patternLength]];
                }
                else
                {
                    i += 1;
                }
                stepCount++;
            }
            else
            {
                // Calculate shift using bad character heuristic
                int badCharShift = j - _badCharTable[text[i + j]];
                i += Math.Max(1, badCharShift);
            }
            stepCount++;
        }

        return occurrences;
    }

    // Build the bad character table
    private int[] BuildBadCharacterTable(string pattern)
    {
        const int ASCII_SIZE = 256;
        int[] table = new int[ASCII_SIZE];
        Array.Fill(table, -1);

        for (int i = 0; i < _patternLength; i++)
        {
            table[(int)pattern[i]] = i;
        }

        return table;
    }
}