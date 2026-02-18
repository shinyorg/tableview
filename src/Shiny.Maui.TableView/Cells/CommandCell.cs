using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

public class CommandCell : LabelCell
{
    private Label _arrowLabel = default!;

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(CommandCell), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(CommandCell), null);

    public static readonly BindableProperty ShowArrowProperty = BindableProperty.Create(
        nameof(ShowArrow), typeof(bool), typeof(CommandCell), true,
        propertyChanged: (b, o, n) => ((CommandCell)b)._arrowLabel.IsVisible = (bool)n);

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool ShowArrow
    {
        get => (bool)GetValue(ShowArrowProperty);
        set => SetValue(ShowArrowProperty, value);
    }

    public CommandCell()
    {
        // Add arrow indicator after the value label
        _arrowLabel = new Label
        {
            Text = "\u203A", // single right-pointing angle
            FontSize = 20,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.Gray,
            Margin = new Thickness(4, 0, 0, 0)
        };

        var accessoryLayout = new HorizontalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            Spacing = 4
        };

        // Move existing ValueLabel into the HorizontalStack
        if (AccessoryView is Label valueLabel)
        {
            var parent = valueLabel.Parent as Layout;
            parent?.Children.Remove(valueLabel);
            accessoryLayout.Children.Add(valueLabel);
        }
        accessoryLayout.Children.Add(_arrowLabel);

        // Replace accessory in grid
        Grid.SetColumn(accessoryLayout, 2);
        Grid.SetRowSpan(accessoryLayout, 2);

        var grid = RootGrid;
        if (AccessoryView != null)
            grid.Children.Remove(AccessoryView);
        grid.Children.Add(accessoryLayout);
    }

    protected override void OnTapped()
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }
}
