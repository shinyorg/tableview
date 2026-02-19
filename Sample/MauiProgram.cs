using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using Shiny.Maui.TableView;
using MauiDevFlow.Agent;

namespace Sample.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseShinyTableView()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.AddMauiDevFlowAgent();
#endif

        return builder.Build();
    }
}
