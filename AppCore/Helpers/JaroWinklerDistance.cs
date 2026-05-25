namespace AppCore.Helpers;

public static class JaroWinklerDistance
{
    public static double Calculate(string s1, string s2)
    {
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2))
            return 0;

        s1 = s1.ToLowerInvariant();
        s2 = s2.ToLowerInvariant();

        if (s1 == s2) return 1;

        int len1 = s1.Length;
        int len2 = s2.Length;
        int matchRange = Math.Max(0, Math.Max(len1, len2) / 2 - 1);

        bool[] s1Matches = new bool[len1];
        bool[] s2Matches = new bool[len2];

        int matches = 0;
        for (int i = 0; i < len1; i++)
        {
            int start = Math.Max(0, i - matchRange);
            int end = Math.Min(i + matchRange + 1, len2);

            for (int j = start; j < end; j++)
            {
                if (!s2Matches[j] && s1[i] == s2[j])
                {
                    s1Matches[i] = true;
                    s2Matches[j] = true;
                    matches++;
                    break;
                }
            }
        }

        if (matches == 0) return 0;

        int transpositions = 0;
        int k = 0;
        for (int i = 0; i < len1; i++)
        {
            if (!s1Matches[i]) continue;
            while (!s2Matches[k]) k++;
            if (s1[i] != s2[k]) transpositions++;
            k++;
        }

        double jaro = (matches / (double)len1 + 
                       matches / (double)len2 + 
                       (matches - transpositions / 2.0) / matches) / 3.0;

        int prefixLength = 0;
        int maxPrefix = Math.Min(4, Math.Min(len1, len2));
        for (int i = 0; i < maxPrefix; i++)
        {
            if (s1[i] == s2[i]) prefixLength++;
            else break;
        }

        return jaro + (prefixLength * 0.1 * (1 - jaro));
    }

    public static bool AreSimilar(string s1, string s2, double threshold = 0.85)
    {
        if (string.IsNullOrEmpty(s1) && string.IsNullOrEmpty(s2)) return true;
        if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return false;
        return Calculate(s1, s2) >= threshold;
    }
}