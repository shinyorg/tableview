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
                AttachDragDrop(cell, section, parentTableView);

            sectionLayout.Children.Add(cell);

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

        var headerColor = section.HeaderBackgroundColor ?? tableView.HeaderBackgroundColor ?? Colors.Transparent;
        var textColor = section.HeaderTextColor ?? tableView.HeaderTextColor ?? Colors.Gray;
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
            BackgroundColor = headerColor,
            Padding = tableView.HeaderPadding
        };

        if (headerHeight >= 0)
            headerContainer.HeightRequest = headerHeight;

        var verticalAlign = headerHeight >= 0
            ? ToLayoutOptions(tableView.HeaderTextVerticalAlign)
            : LayoutOptions.Center;

        var headerLabel = new Label
        {
            Text = section.Title,
            TextColor = textColor,
            FontSize = fontSize,
            FontAttributes = fontAttributes,
            VerticalOptions = verticalAlign
        };

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

        var bgColor = section.FooterBackgroundColor ?? tableView.FooterBackgroundColor ?? Colors.Transparent;
        var textColor = section.FooterTextColor ?? tableView.FooterTextColor ?? Colors.Gray;
        var fontSize = section.FooterFontSize >= 0 ? section.FooterFontSize
            : tableView.FooterFontSize >= 0 ? tableView.FooterFontSize
            : 12;
        var fontFamily = section.FooterFontFamily ?? tableView.FooterFontFamily;
        var fontAttributes = section.FooterFontAttributes ?? tableView.FooterFontAttributes;

        var footerContainer = new ContentView
        {
            Padding = tableView.FooterPadding,
            BackgroundColor = bgColor
        };

        var footerLabel = new Label
        {
            Text = section.FooterText,
            TextColor = textColor,
            FontSize = fontSize,
            FontAttributes = fontAttributes
        };

        if (fontFamily != null)
            footerLabel.FontFamily = fontFamily;

        footerContainer.Content = footerLabel;
        layout.Children.Add(footerContainer);
    }

    private static BoxView CreateSeparator(TvTableView tableView)
    {
        return new BoxView
        {
            HeightRequest = tableView.SeparatorHeight >= 0 ? tableView.SeparatorHeight : 0.5,
            Color = tableView.SeparatorColor ?? Colors.LightGray,
            Margin = new Thickness(tableView.SeparatorPadding >= 0 ? tableView.SeparatorPadding : 16, 0, 0, 0)
        };
    }

    private static void AttachDragDrop(CellBase cell, TvTableSection section, TvTableView tableView)
    {
        var drag = new DragGestureRecognizer { CanDrag = true };
        drag.DragStarting += (s, e) =>
        {
            e.Data.Properties["cell"] = cell;
            e.Data.Properties["section"] = section;
        };
        cell.GestureRecognizers.Add(drag);

        var drop = new DropGestureRecognizer { AllowDrop = true };
        drop.Drop += (s, e) =>
        {
            if (e.Data.Properties.TryGetValue("cell", out var draggedObj) &&
                e.Data.Properties.TryGetValue("section", out var srcSectionObj) &&
                draggedObj is CellBase draggedCell &&
                srcSectionObj is TvTableSection srcSection &&
                srcSection == section)
            {
                var fromIndex = section.Cells.IndexOf(draggedCell);
                var toIndex = section.Cells.IndexOf(cell);

                if (fromIndex >= 0 && toIndex >= 0 && fromIndex != toIndex)
                {
                    section.Cells.Move(fromIndex, toIndex);
                    tableView.RaiseItemDropped(section, draggedCell, fromIndex, toIndex);
                }
            }
        };
        cell.GestureRecognizers.Add(drop);
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
