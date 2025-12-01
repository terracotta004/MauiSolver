using System.IO;
using System.Linq;
using Microsoft.Maui.Storage;

namespace MauiSolver.CryptogramSolver;

public class Corpus
{
    private readonly Dictionary<string, List<string>> _hashDict = new();

    public Corpus()
    {
        List<string> wordList = new();

        try
        {
            using var stream = FileSystem.OpenAppPackageFileAsync("corpus.txt").GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            var content = reader.ReadToEndAsync().GetAwaiter().GetResult();
            wordList = content
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        foreach (var word in wordList)
        {
            var wordHash = WordHasher.HashWord(word);
            if (!_hashDict.TryGetValue(wordHash, out var list))
            {
                list = new List<string>();
                _hashDict[wordHash] = list;
            }
            list.Add(word);
        }
    }

    /// <summary>
    /// Finds words in the corpus that could match the given word in ciphertext.
    /// Uppercase letters = ciphertext; lowercase letters = plaintext.
    /// </summary>
    public List<string> FindCandidates(string inputWord)
    {
        var inputWordHash = WordHasher.HashWord(inputWord);
        _hashDict.TryGetValue(inputWordHash, out var matchesHash);
        matchesHash ??= new List<string>();

        var candidates = new List<string>();

        foreach (var word in matchesHash)
        {
            bool invalid = false;

            for (int i = 0; i < word.Length; i++)
            {
                char inChar = inputWord[i];
                char corpusChar = word[i];

                // If either is lowercase or apostrophe, they must match exactly.
                if ((char.IsLower(inChar) || inChar == '\'' || corpusChar == '\'')
                    && inChar != corpusChar)
                {
                    invalid = true;
                    break;
                }
            }

            if (!invalid)
            {
                candidates.Add(word);
            }
        }

        return candidates;
    }
}
