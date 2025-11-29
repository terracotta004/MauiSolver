using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MauiSolver.Services;

public sealed class CryptogramSolver : ICryptogramSolver
{
    private readonly ILogger<CryptogramSolver> _logger;
    private readonly string _solverPath;
    private readonly string _corpusPath;

    public CryptogramSolver(ILogger<CryptogramSolver> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _solverPath = Path.Combine(AppContext.BaseDirectory, "sub_solver.exe");
        _corpusPath = Path.Combine(AppContext.BaseDirectory, "corpus.txt");
    }

    public async Task<string> SolveAsync(string input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input text is required.", nameof(input));

        EnsureDependenciesPresent();

        var inputPath = Path.Combine(Path.GetTempPath(), $"cryptogram_in_{Guid.NewGuid():N}.txt");

        try
        {
            await File.WriteAllTextAsync(inputPath, input, cancellationToken).ConfigureAwait(false);
            return await RunProcessAsync(inputPath, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            TryDelete(inputPath);
        }
    }

    private void EnsureDependenciesPresent()
    {
        if (!File.Exists(_solverPath))
            throw new FileNotFoundException("Cryptogram solver binary not found.", _solverPath);

        if (!File.Exists(_corpusPath))
            throw new FileNotFoundException("Corpus file not found.", _corpusPath);
    }

    private async Task<string> RunProcessAsync(string inputPath, CancellationToken cancellationToken)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = _solverPath,
            Arguments = $"\"{inputPath}\" -c \"{_corpusPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (_, args) =>
        {
            if (args.Data != null)
                outputBuilder.AppendLine(args.Data);
        };

        process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data != null)
                errorBuilder.AppendLine(args.Data);
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);

        if (process.ExitCode != 0)
        {
            var error = errorBuilder.ToString();
            _logger.LogError("Cryptogram solver failed with exit code {ExitCode}: {Error}", process.ExitCode, error);
            throw new InvalidOperationException($"Cryptogram solver failed with exit code {process.ExitCode}.");
        }

        var output = outputBuilder.ToString();
        if (string.IsNullOrWhiteSpace(output))
        {
            var error = errorBuilder.ToString();
            _logger.LogWarning("Cryptogram solver produced no output. Error stream: {Error}", error);
            return string.Empty;
        }

        return output;
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // We intentionally swallow clean-up failures; they are not critical for the solver result.
        }
    }
}
