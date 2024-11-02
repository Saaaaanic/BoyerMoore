namespace BoyerMoore;

public interface ISubstringSearch
{
    List<int> Search(string text, string pattern, out long stepCount);
}