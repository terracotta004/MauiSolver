using IronPython.Hosting;
using Microsoft.Maui.Controls;
using System.Diagnostics;

namespace MauiSolver
{
    public partial class SolveCryptogramPage : ContentPage
    {

        public SolveCryptogramPage()
        {
            InitializeComponent();
        }

        private void OnSubmitButtonClicked(object sender, EventArgs e)
        {
            var input_text = TextEditor.Text;

            var infile_path = "infile" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            var outfile_path = "outfile" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            File.WriteAllText(infile_path, input_text);

            var file = File.ReadAllText(infile_path);

            //var process1 = Process.Start("cmd", " /k sub_solver.exe " + infile_path + " -c corpus.txt");

            var process2 = Process.Start("cmd", " /k sub_solver.exe " + infile_path + " -c corpus.txt > " + outfile_path);

            //var process3 = Process.Start("sub_solver.exe", infile_path + " -c corpus.txt > " + outfile_path);

            //var process4 = Process.Start("start", "sub_solver.exe " + infile_path + " -c corpus.txt > " + outfile_path);

            process2.WaitForExit();
            //process4.WaitForExit();

            Thread.Sleep(3000);

            var outfile_exists = File.Exists(outfile_path);

            Thread.Sleep(3000);

            DisplayLabel.Text = File.ReadAllText(outfile_path);

            Thread.Sleep(3000);

            File.Delete(infile_path);
            File.Delete(outfile_path);

        }
    }
}