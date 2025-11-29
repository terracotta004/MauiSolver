using System.Threading;

namespace MauiSolver.Services;

public sealed class BacktrackingSudokuSolver : ISudokuSolver
{
    public bool TrySolve(int[,] board, out int[,] solvedBoard, CancellationToken cancellationToken = default)
    {
        if (board == null)
            throw new ArgumentNullException(nameof(board));

        solvedBoard = (int[,])board.Clone();
        return SolveCell(solvedBoard, 0, 0, cancellationToken);
    }

    private bool SolveCell(int[,] board, int row, int col, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (row == 9)
            return true;

        var nextRow = col == 8 ? row + 1 : row;
        var nextCol = (col + 1) % 9;

        if (board[row, col] != 0)
            return SolveCell(board, nextRow, nextCol, cancellationToken);

        for (int num = 1; num <= 9; num++)
        {
            if (IsValid(board, row, col, num))
            {
                board[row, col] = num;
                if (SolveCell(board, nextRow, nextCol, cancellationToken))
                    return true;
                board[row, col] = 0;
            }
        }

        return false;
    }

    private static bool IsValid(int[,] board, int row, int col, int num)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == num || board[i, col] == num)
                return false;
        }

        int startRow = (row / 3) * 3;
        int startCol = (col / 3) * 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[startRow + i, startCol + j] == num)
                    return false;
            }
        }

        return true;
    }
}
