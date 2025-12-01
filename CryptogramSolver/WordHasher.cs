namespace MauiSolver.CryptogramSolver;

    public static class WordHasher
    {
        /// <summary>
        /// Hashes a word into its similarity equivalent.
        /// MXM -> 010, ASDF -> 0123, AFAFA -> 01010, etc.
        /// </summary>
        public static string HashWord(string word)
        {
            var seen = new Dictionary<char, int>();
            var output = new List<int>();
            var index = 0;

            foreach (var c in word)
            {
                if (!seen.ContainsKey(c))
                {
                    seen[c] = index;
                    index++;
                }
                output.Add(seen[c]);
            }

            return string.Join(string.Empty, output);
        }
    }
