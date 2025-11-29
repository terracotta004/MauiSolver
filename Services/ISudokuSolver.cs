using System.Threading;

namespace MauiSolver.Services;

public interface ISudokuSolver
{
    bool TrySolve(int[,] board, out int[,] solvedBoard, CancellationToken cancellationToken = default);
}
