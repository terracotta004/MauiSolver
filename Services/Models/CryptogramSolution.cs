using System.Text;

namespace MauiSolver.Services.Models;

public sealed class CryptogramSolution
{
    public string Ciphertext { get; }
    public string Plaintext { get; }
    public IReadOnlyDictionary<char, char> Substitutions { get; }

    public CryptogramSolution(string ciphertext, string plaintext, IReadOnlyDictionary<char, char> substitutions)
    {
        Ciphertext = ciphertext ?? string.Empty;
        Plaintext = plaintext ?? string.Empty;
        Substitutions = substitutions ?? new Dictionary<char, char>();
    }

    public string ToReport()
    {
        var builder = new StringBuilder();
        builder.AppendLine("Ciphertext:");
        builder.AppendLine(Ciphertext);
        builder.AppendLine();
        builder.AppendLine("Plaintext:");
        builder.AppendLine(Plaintext);
        builder.AppendLine();
        builder.AppendLine("Substitutions:");

        var items = Substitutions
            .Select(kv => $"{kv.Key} -> {kv.Value}")
            .OrderBy(s => s)
            .ToList();

        for (int i = 0; i < items.Count; i++)
        {
            builder.Append(items[i]);
            builder.Append(' ');
            if (i % 5 == 4)
            {
                builder.AppendLine();
            }
        }

        builder.AppendLine();
        return builder.ToString();
    }
}
