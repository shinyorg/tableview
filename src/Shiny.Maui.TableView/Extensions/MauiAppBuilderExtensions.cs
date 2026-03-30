using Microsoft.Maui.Handlers;
using Shiny.Maui.TableView.Controls;

namespace Shiny.Maui.TableView;

public static class MauiAppBuilderExtensions
{
    public static MauiAppBuilder UseShinyTableView(this MauiAppBuilder builder)
    {
        EntryHandler.Mapper.AppendToMapping("ShinyBorderless", (handler, view) =>
        {
            if (view is not BorderlessEntry)
                return;

#if ANDROID
            handler.PlatformView.Background = null;
#elif IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
        });

        return builder;
    }
}
