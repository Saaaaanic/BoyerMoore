namespace BoyerMoore;

public class NaiveSubstringSearch : ISubstringSearch
{
    public List<int> Search(string text, string pattern, out long stepCount)
    {
        int textLength = text.Length;
        stepCount = 0;
        int patternLength = pattern.Length;
        List<int> matches = new List<int>();

        for (int i = 0; i <= textLength - patternLength; i++)
        {
            stepCount++; // Step: Outer loop iteration
            bool match = true;
            for (int j = 0; j < patternLength; j++)
            {
                stepCount++; // Step: Inner loop iteration
                if (text[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }

            if (match)
            {
                matches.Add(i);
            }
        }

        return matches;
    }
}