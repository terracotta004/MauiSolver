using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MauiSolver.CryptogramSolver;
using MauiSolver.Services.Models;
using Microsoft.Extensions.Logging;

namespace MauiSolver.Services;

public sealed class CryptogramSolver : ICryptogramSolver
{
    private readonly ILogger<CryptogramSolver> _logger;

    public CryptogramSolver(ILogger<CryptogramSolver> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<CryptogramSolution> SolveAsync(string input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input text is required.", nameof(input));

        // var corpusPath = Path.Combine(AppContext.BaseDirectory, "corpus.txt");
        // if (!File.Exists(corpusPath))
            // throw new FileNotFoundException("Corpus file not found.", corpusPath);

        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            var solver = new SubSolver(input);
            var report = CaptureReport(solver, cancellationToken);
            var solution = ParseReport(report, solver.Ciphertext);

            if (string.IsNullOrWhiteSpace(solution.Plaintext))
            {
                _logger.LogWarning("Cryptogram solver produced no plaintext output.");
            }

            return solution;
        }, cancellationToken).ConfigureAwait(false);
    }

    private static string CaptureReport(SubSolver solver, CancellationToken cancellationToken)
    {
        var reportBuilder = new StringBuilder();
        var originalOut = Console.Out;
        using var writer = new StringWriter(reportBuilder);
        try
        {
            Console.SetOut(writer);
            solver.Solve();
            cancellationToken.ThrowIfCancellationRequested();
            solver.PrintReport();
            writer.Flush();
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        return reportBuilder.ToString();
    }

    private static CryptogramSolution ParseReport(string report, string ciphertext)
    {
        var lines = report.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        string plaintext = string.Empty;
        var substitutions = new Dictionary<char, char>();

        // Extract plaintext: line after "Plaintext:" (first non-empty).
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Trim().Equals("Plaintext:", StringComparison.OrdinalIgnoreCase))
            {
                for (int j = i + 1; j < lines.Length; j++)
                {
                    if (!string.IsNullOrWhiteSpace(lines[j]))
                    {
                        plaintext = lines[j].Trim();
                        break;
                    }
                }
                break;
            }
        }

        // Extract substitutions: collect tokens after "Substitutions:".
        var tokens = new List<string>();
        bool inSubs = false;
        foreach (var line in lines)
        {
            if (line.Trim().Equals("Substitutions:", StringComparison.OrdinalIgnoreCase))
            {
                inSubs = true;
                continue;
            }

            if (!inSubs)
                continue;

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            tokens.AddRange(parts);
        }

        for (int i = 0; i + 2 < tokens.Count; i += 3)
        {
            var from = tokens[i];
            var arrow = tokens[i + 1];
            var to = tokens[i + 2];

            if (arrow == "->" && from.Length == 1 && to.Length == 1)
            {
                substitutions[from[0]] = to[0];
            }
        }

        return new CryptogramSolution(ciphertext ?? string.Empty, plaintext, substitutions);
    }
}
