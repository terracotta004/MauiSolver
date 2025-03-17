namespace MauiSolver;

public partial class SolveSudokuPage : ContentPage
{
	public SolveSudokuPage()
	{
		InitializeComponent();
        CreateSudokuGrid();
    }

    private Entry[,] _entries = new Entry[9, 9];

    private void CreateSudokuGrid()
    {
        Grid grid = new Grid { Padding = 10 };
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

    private void SolveSudoku(object sender, EventArgs e)
    {
        int[,] board = new int[9, 9];
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (int.TryParse(_entries[row, col].Text, out int num))
                    board[row, col] = num;
                else
                    board[row, col] = 0;
            }
        }

        if (Solve(board))
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    _entries[row, col].Text = board[row, col].ToString();
                }
            }
        }
        else
        {
            DisplayAlert("Error", "No solution exists for this Sudoku.", "OK");
        }
    }

    private bool Solve(int[,] board)
    {
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (board[row, col] == 0)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        if (IsValid(board, row, col, num))
                        {
                            board[row, col] = num;
                            if (Solve(board))
                                return true;
                            board[row, col] = 0;
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }

    private bool IsValid(int[,] board, int row, int col, int num)
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
