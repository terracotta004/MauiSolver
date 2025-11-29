using System.Threading;
using System.Threading.Tasks;

namespace MauiSolver.Services;

public interface ICryptogramSolver
{
    Task<string> SolveAsync(string input, CancellationToken cancellationToken = default);
}
