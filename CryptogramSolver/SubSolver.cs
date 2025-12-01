using MauiSolver.Services.Models;
using System.Text.RegularExpressions;

namespace MauiSolver.CryptogramSolver;
public class SubSolver
    {
        private readonly Corpus _corpus;
        private Dictionary<char, char> _translation = new();
        public string Ciphertext { get; }
        public bool Verbose { get; }

        public SubSolver(string ciphertext, bool verbose = false)
        {
            _corpus = new Corpus();
            Ciphertext = ciphertext.ToUpperInvariant();
            Verbose = verbose;
        }

        public void Solve()
        {
            // Strip non-word, non-space chars and split to words.
            var cleaned = Regex.Replace(Ciphertext, @"[^\w ]+", string.Empty);
            var words = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            // Sort by descending length.
            words.Sort((a, b) => b.Length.CompareTo(a.Length));

            // Try increasing thresholds for unknown words.
            int maxIterations = Math.Max(3, words.Count / 10);
            for (int maxUnknownWordCount = 0; maxUnknownWordCount < maxIterations; maxUnknownWordCount++)
            {
                var solution = RecursiveSolve(words, new Dictionary<char, char>(), 0, maxUnknownWordCount);
                if (solution != null)
                {
                    _translation = solution;
                    break;
                }
            }
        }

        private Dictionary<char, char>? RecursiveSolve(
            IList<string> remainingWords,
            Dictionary<char, char> currentTranslation,
            int unknownWordCount,
            int maxUnknownWordCount)
        {
            var trans = MakeTransFromDict(currentTranslation);

            if (Verbose)
            {
                Console.WriteLine(ApplyTranslation(Ciphertext, trans));
            }

            if (remainingWords.Count == 0)
            {
                return currentTranslation;
            }

            if (unknownWordCount > maxUnknownWordCount)
            {
                return null;
            }

            var cipherWord = remainingWords[0];
            var translatedCipherWord = ApplyTranslation(cipherWord, trans);

            var candidates = _corpus.FindCandidates(translatedCipherWord);

            foreach (var candidate in candidates)
            {
                var newTrans = new Dictionary<char, char>(currentTranslation);
                var translatedPlaintextChars = new HashSet<char>(currentTranslation.Values);
                bool badTranslation = false;

                for (int i = 0; i < candidate.Length; i++)
                {
                    char cipherChar = cipherWord[i];
                    char plainChar = candidate[i];

                    // Bad if we try to map an unseen cipher char to a plaintext char
                    // that is already mapped from a different cipher char.
                    if (!currentTranslation.ContainsKey(cipherChar)
                        && translatedPlaintextChars.Contains(plainChar))
                    {
                        badTranslation = true;
                        break;
                    }

                    newTrans[cipherChar] = plainChar;
                }

                if (badTranslation)
                {
                    continue;
                }

                var subRemaining = remainingWords.Skip(1).ToList();
                var result = RecursiveSolve(subRemaining, newTrans, unknownWordCount, maxUnknownWordCount);
                if (result != null)
                {
                    return result;
                }
            }

            // Try skipping this word (could be a proper noun not in corpus).
            {
                var subRemaining = remainingWords.Skip(1).ToList();
                var skipWordSolution = RecursiveSolve(
                    subRemaining,
                    currentTranslation,
                    unknownWordCount + 1,
                    maxUnknownWordCount);

                if (skipWordSolution != null)
                {
                    return skipWordSolution;
                }
            }

            return null;
        }

        private static Dictionary<char, char> MakeTransFromDict(Dictionary<char, char> translations)
        {
            // In Python this returns a mapping usable with string.translate().
            // Here we just return a copy (dictionary itself is the mapping).
            return new Dictionary<char, char>(translations);
        }

        private static string ApplyTranslation(string input, Dictionary<char, char> trans)
        {
            var chars = input.ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (trans.TryGetValue(chars[i], out var mapped))
                {
                    chars[i] = mapped;
                }
            }
            return new string(chars);
        }

        public void PrintReport()
        {
            if (_translation == null || _translation.Count == 0)
            {
                Console.WriteLine("Failed to translate ciphertext.");
                return;
            }

            var trans = MakeTransFromDict(_translation);
            var plaintext = ApplyTranslation(Ciphertext, trans);

            Console.WriteLine("Ciphertext:");
            Console.WriteLine(Ciphertext);
            Console.WriteLine();

            Console.WriteLine("Plaintext:");
            Console.WriteLine(plaintext);
            Console.WriteLine();

            Console.WriteLine("Substitutions:");
            var items = _translation
                .Select(kv => $"{kv.Key} -> {kv.Value}")
                .OrderBy(s => s)
                .ToList();

            for (int i = 0; i < items.Count; i++)
            {
                Console.Write(items[i] + " ");
                if (i % 5 == 4)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }
    }
