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

        var headerContainer = new ContentView
        {
            BackgroundColor = headerColor,
            Padding = new Thickness(tableView.HeaderPadding >= 0 ? tableView.HeaderPadding : 16, 8)
        };

        var headerLabel = new Label
        {
            Text = section.Title,
            TextColor = textColor,
            FontSize = fontSize,
            FontAttributes = FontAttributes.Bold
        };

        headerContainer.Content = headerLabel;
        layout.Children.Add(headerContainer);
    }

    private static void RenderFooter(VerticalStackLayout layout, TvTableSection section, TvTableView tableView)
    {
        if (section.FooterView != null)
        {
            layout.Children.Add(section.FooterView);
            return;
        }

        if (string.IsNullOrEmpty(section.FooterText))
            return;

        var textColor = section.FooterTextColor ?? tableView.FooterTextColor ?? Colors.Gray;
        var fontSize = section.FooterFontSize >= 0 ? section.FooterFontSize
            : tableView.FooterFontSize >= 0 ? tableView.FooterFontSize
            : 12;

        var footerContainer = new ContentView
        {
            Padding = new Thickness(tableView.FooterPadding >= 0 ? tableView.FooterPadding : 16, 4, 16, 8)
        };

        var footerLabel = new Label
        {
            Text = section.FooterText,
            TextColor = textColor,
            FontSize = fontSize
        };

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
                    tableView.RaiseItemDropped(section, fromIndex, toIndex);
                }
            }
        };
        cell.GestureRecognizers.Add(drop);
    }
}
