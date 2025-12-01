using System.Threading;
using System.Threading.Tasks;
using MauiSolver.Services.Models;

namespace MauiSolver.Services;

public interface ICryptogramSolver
{
    Task<CryptogramSolution> SolveAsync(string input, CancellationToken cancellationToken = default);
}
