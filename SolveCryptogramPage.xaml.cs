using System.Diagnostics;
using MauiSolver.Services;

namespace MauiSolver;

public partial class SolveCryptogramPage : ContentPage
{
    private readonly ICryptogramSolver _cryptogramSolver;

    public SolveCryptogramPage(ICryptogramSolver cryptogramSolver)
    {
        _cryptogramSolver = cryptogramSolver ?? throw new ArgumentNullException(nameof(cryptogramSolver));
        InitializeComponent();
    }

    private async Task RunSubSolverAsync()
    {
        var inputText = TextEditor.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(inputText))
        {
            await DisplayAlertAsync("Missing input", "Please enter text to solve.", "OK");
            return;
        }

        try
        {
            DisplayLabel.Text = "Solving...";
            var result = await _cryptogramSolver.SolveAsync(inputText);
            DisplayLabel.Text = string.IsNullOrWhiteSpace(result) ? "No output returned." : result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await DisplayAlertAsync("Error", "Unable to solve the cryptogram. See logs for details.", "OK");
        }
    }

    private async void OnSubmitButtonClicked(object sender, EventArgs e)
    {
        await RunSubSolverAsync();
    }
}
