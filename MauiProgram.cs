using MauiSolver.Services;
using Microsoft.Extensions.Logging;

namespace MauiSolver;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<ICryptogramSolver, CryptogramSolver>();
        builder.Services.AddSingleton<ISudokuSolver, BacktrackingSudokuSolver>();
        builder.Services.AddTransient<SolveCryptogramPage>();
        builder.Services.AddTransient<SolveSudokuPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
