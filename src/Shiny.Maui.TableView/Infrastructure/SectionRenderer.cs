using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.TableView.Cells;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;
using TvTableSection = Shiny.Maui.TableView.Sections.TableSection;

namespace Shiny.Maui.TableView.Infrastructure;

internal static class SectionRenderer
{
    public static View Render(TvTableSection section, TvTableView parentTableView)
    {
        if (!section.IsVisible)
            return new ContentView { IsVisible = false };

        var sectionLayout = new VerticalStackLayout();

        // Header
        RenderHeader(sectionLayout, section, parentTableView);

        // Cells with separators
        var cells = section.GetVisibleCells();
        for (int i = 0; i < cells.Count; i++)
        {
            var cell = cells[i];
            cell.ParentTableView = parentTableView;
            cell.ParentSection = section;
            cell.ApplyCascadedStyles();

            if (section.UseDragSort)
            {
                var wrappedCell = WrapWithReorderControls(cell, section, parentTableView);
                sectionLayout.Children.Add(wrappedCell);
            }
            else
            {
                sectionLayout.Children.Add(cell);
            }

            // Separator between cells (not after last)
            if (i < cells.Count - 1)
            {
                sectionLayout.Children.Add(CreateSeparator(parentTableView));
            }
        }

        // Footer
        RenderFooter(sectionLayout, section, parentTableView);

        return sectionLayout;
    }

    private static void RenderHeader(VerticalStackLayout layout, TvTableSection section, TvTableView tableView)
    {
        if (section.HeaderView != null)
        {
            layout.Children.Add(section.HeaderView);
            return;
        }

        if (string.IsNullOrEmpty(section.Title))
            return;

        var headerColor = section.HeaderBackgroundColor ?? tableView.HeaderBackgroundColor;
        var textColor = section.HeaderTextColor ?? tableView.HeaderTextColor;
        var fontSize = section.HeaderFontSize >= 0 ? section.HeaderFontSize
            : tableView.HeaderFontSize >= 0 ? tableView.HeaderFontSize
            : 14;
        var fontFamily = section.HeaderFontFamily ?? tableView.HeaderFontFamily;
        var fontAttributes = section.HeaderFontAttributes ?? tableView.HeaderFontAttributes;

        var headerHeight = section.HeaderHeight >= 0 ? section.HeaderHeight
            : tableView.HeaderHeight >= 0 ? tableView.HeaderHeight
            : -1;

        var headerContainer = new ContentView
        {
            Padding = tableView.HeaderPadding
        };
        if (headerColor != null)
            headerContainer.BackgroundColor = headerColor;

        if (headerHeight >= 0)
            headerContainer.HeightRequest = headerHeight;

        var verticalAlign = headerHeight >= 0
            ? ToLayoutOptions(tableView.HeaderTextVerticalAlign)
            : LayoutOptions.Center;

        var headerLabel = new Label
        {
            Text = section.Title,
            FontSize = fontSize,
            FontAttributes = fontAttributes,
            VerticalOptions = verticalAlign
        };
        if (textColor != null)
            headerLabel.TextColor = textColor;

        if (fontFamily != null)
            headerLabel.FontFamily = fontFamily;

        headerContainer.Content = headerLabel;
        layout.Children.Add(headerContainer);
    }

    private static void RenderFooter(VerticalStackLayout layout, TvTableSection section, TvTableView tableView)
    {
        if (!section.FooterVisible)
            return;

        if (section.FooterView != null)
        {
            layout.Children.Add(section.FooterView);
            return;
        }

        if (string.IsNullOrEmpty(section.FooterText))
            return;

        var bgColor = section.FooterBackgroundColor ?? tableView.FooterBackgroundColor;
        var textColor = section.FooterTextColor ?? tableView.FooterTextColor;
        var fontSize = section.FooterFontSize >= 0 ? section.FooterFontSize
            : tableView.FooterFontSize >= 0 ? tableView.FooterFontSize
            : 12;
        var fontFamily = section.FooterFontFamily ?? tableView.FooterFontFamily;
        var fontAttributes = section.FooterFontAttributes ?? tableView.FooterFontAttributes;

        var footerContainer = new ContentView
        {
            Padding = tableView.FooterPadding
        };
        if (bgColor != null)
            footerContainer.BackgroundColor = bgColor;

        var footerLabel = new Label
        {
            Text = section.FooterText,
            FontSize = fontSize,
            FontAttributes = fontAttributes
        };
        if (textColor != null)
            footerLabel.TextColor = textColor;

        if (fontFamily != null)
            footerLabel.FontFamily = fontFamily;

        footerContainer.Content = footerLabel;
        layout.Children.Add(footerContainer);
    }

    private static BoxView CreateSeparator(TvTableView tableView)
    {
        var separator = new BoxView
        {
            HeightRequest = tableView.SeparatorHeight >= 0 ? tableView.SeparatorHeight : 0.5,
            Margin = new Thickness(tableView.SeparatorPadding >= 0 ? tableView.SeparatorPadding : 16, 0, 0, 0),
            Opacity = 0.2
        };
        if (tableView.SeparatorColor != null)
        {
            separator.Color = tableView.SeparatorColor;
            separator.Opacity = 1;
        }
        return separator;
    }

    private static View WrapWithReorderControls(CellBase cell, TvTableSection section, TvTableView tableView)
    {
        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto)
            }
        };

        Grid.SetColumn(cell, 0);
        grid.Children.Add(cell);

        var upLabel = new Label
        {
            Text = "\u25B2",
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(8, 4),
            Opacity = 0.5
        };
        var upTap = new TapGestureRecognizer();
        upTap.Tapped += (s, e) =>
        {
            var index = section.Cells.IndexOf(cell);
            if (index > 0)
                MoveCell(tableView, section, cell, index, index - 1);
        };
        upLabel.GestureRecognizers.Add(upTap);

        var downLabel = new Label
        {
            Text = "\u25BC",
            FontSize = 12,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(8, 4),
            Opacity = 0.5
        };
        var downTap = new TapGestureRecognizer();
        downTap.Tapped += (s, e) =>
        {
            var index = section.Cells.IndexOf(cell);
            if (index >= 0 && index < section.Cells.Count - 1)
                MoveCell(tableView, section, cell, index, index + 1);
        };
        downLabel.GestureRecognizers.Add(downTap);

        var buttonStack = new VerticalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            Spacing = 0,
            Padding = new Thickness(4, 0, 8, 0)
        };
        buttonStack.Children.Add(upLabel);
        buttonStack.Children.Add(downLabel);

        Grid.SetColumn(buttonStack, 1);
        grid.Children.Add(buttonStack);

        return grid;
    }

    private static void MoveCell(TvTableView tableView, TvTableSection section, CellBase cell, int fromIndex, int toIndex)
    {
        tableView.SuppressRender = true;
        section.Cells.Move(fromIndex, toIndex);
        tableView.SuppressRender = false;

        // Defer re-render to next frame so the current touch event completes first
        tableView.Dispatcher.Dispatch(() =>
        {
            tableView.RenderSections();
            tableView.RaiseItemDropped(section, cell, fromIndex, toIndex);
        });
    }

    private static LayoutOptions ToLayoutOptions(LayoutAlignment alignment) => alignment switch
    {
        LayoutAlignment.Start => LayoutOptions.Start,
        LayoutAlignment.Center => LayoutOptions.Center,
        LayoutAlignment.End => LayoutOptions.End,
        LayoutAlignment.Fill => LayoutOptions.Fill,
        _ => LayoutOptions.End
    };
}
