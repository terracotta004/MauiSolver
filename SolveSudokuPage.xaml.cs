using MauiSolver.Services;

namespace MauiSolver;

public partial class SolveSudokuPage : ContentPage
{
    private readonly ISudokuSolver _sudokuSolver;
    private readonly Entry[,] _entries = new Entry[9, 9];

    public SolveSudokuPage(ISudokuSolver sudokuSolver)
    {
        _sudokuSolver = sudokuSolver ?? throw new ArgumentNullException(nameof(sudokuSolver));
        InitializeComponent();
        CreateSudokuGrid();
    }

    private void CreateSudokuGrid()
    {
        var grid = new Grid { Padding = 10 };
        for (int i = 0; i < 9; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        }

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                var entry = new Entry
                {
                    WidthRequest = 40,
                    Keyboard = Keyboard.Numeric,
                    Text = "",
                    FontSize = 20,
                    HorizontalTextAlignment = TextAlignment.Center
                };
                _entries[row, col] = entry;
                Grid.SetRow(entry, row);
                Grid.SetColumn(entry, col);
                grid.Children.Add(entry);
            }
        }

        var solveButton = new Button { Text = "Solve Sudoku" };
        solveButton.Clicked += SolveSudoku;
        var layout = new StackLayout
        {
            Children = { grid, solveButton },
            Spacing = 10,
            Padding = 10
        };

        Content = layout;
    }

    private void SolveSudoku(object? sender, EventArgs e)
    {
        var board = ReadBoard();
        if (_sudokuSolver.TrySolve(board, out var solvedBoard))
        {
            WriteBoard(solvedBoard);
        }
        else
        {
            DisplayAlert("Error", "No solution exists for this Sudoku.", "OK");
        }
    }

    private int[,] ReadBoard()
    {
        var board = new int[9, 9];
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (int.TryParse(_entries[row, col].Text, out var num))
                    board[row, col] = num;
            }
        }

        return board;
    }

    private void WriteBoard(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                _entries[row, col].Text = board[row, col].ToString();
            }
        }
    }
}
