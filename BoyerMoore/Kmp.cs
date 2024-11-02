namespace BoyerMoore;

public class Kmp : ISubstringSearch
{
    private static int[] ComputeLps(string pattern, ref long stepCount)
    {
        int m = pattern.Length;
        int[] lps = new int[m];
        int length = 0;
        int i = 1;

        while (i < m)
        {
            if (pattern[i] == pattern[length])
            {
                length++;
                lps[i] = length;
                i++;
            }
            else
            {
                if (length != 0)
                {
                    length = lps[length - 1];
                }
                else
                {
                    lps[i] = 0;
                    i++;
                }
            }
            stepCount++;
        }
        return lps;
    }

    public List<int> Search(string text, string pattern, out long stepCount)
    {
        int n = text.Length;
        stepCount = 0;
        int m = pattern.Length;
        int[] lps = ComputeLps(pattern, ref stepCount);
        int i = 0;
        int j = 0;
        List<int> result = new List<int>();

        while (i < n)
        {
            if (text[i] == pattern[j])
            {
                i++;
                j++;
            }

            if (j == m)
            {
                result.Add(i - j);
                j = lps[j - 1];
            }
            else if (i < n && text[i] != pattern[j])
            {
                if (j != 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
            stepCount++;
        }

        return result;
    }
}