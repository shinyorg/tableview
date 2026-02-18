using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

[ContentProperty(nameof(CustomContent))]
public class CustomCell : CellBase
{
    public static readonly BindableProperty CustomContentProperty = BindableProperty.Create(
        nameof(CustomContent), typeof(View), typeof(CustomCell), null,
        propertyChanged: OnCustomContentChanged);

    public static readonly BindableProperty UseFullSizeProperty = BindableProperty.Create(
        nameof(UseFullSize), typeof(bool), typeof(CustomCell), false,
        propertyChanged: (b, o, n) => ((CustomCell)b).UpdateLayout());

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(CustomCell), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(CustomCell), null);

    public static readonly BindableProperty LongCommandProperty = BindableProperty.Create(
        nameof(LongCommand), typeof(ICommand), typeof(CustomCell), null);

    public static readonly BindableProperty LongCommandParameterProperty = BindableProperty.Create(
        nameof(LongCommandParameter), typeof(object), typeof(CustomCell), null);

    public static readonly BindableProperty ShowArrowProperty = BindableProperty.Create(
        nameof(ShowArrow), typeof(bool), typeof(CustomCell), false,
        propertyChanged: (b, o, n) => ((CustomCell)b).UpdateArrowVisibility());

    private Label? _arrowLabel;

    public View? CustomContent
    {
        get => (View?)GetValue(CustomContentProperty);
        set => SetValue(CustomContentProperty, value);
    }

    public bool UseFullSize
    {
        get => (bool)GetValue(UseFullSizeProperty);
        set => SetValue(UseFullSizeProperty, value);
    }

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

    public ICommand? LongCommand
    {
        get => (ICommand?)GetValue(LongCommandProperty);
        set => SetValue(LongCommandProperty, value);
    }

    public object? LongCommandParameter
    {
        get => GetValue(LongCommandParameterProperty);
        set => SetValue(LongCommandParameterProperty, value);
    }

    public bool ShowArrow
    {
        get => (bool)GetValue(ShowArrowProperty);
        set => SetValue(ShowArrowProperty, value);
    }

    public CustomCell()
    {
        // Add long press gesture
        var longPress = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
        longPress.Tapped += OnLongPressed;
        GestureRecognizers.Add(longPress);
    }

    private static void OnCustomContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((CustomCell)bindable).UpdateLayout();
    }

    private void UpdateLayout()
    {
        if (UseFullSize && CustomContent != null)
        {
            // Replace entire cell content with custom content
            Content = CustomContent;
        }
        else
        {
            UpdateAccessoryContent();
        }
    }

    private void UpdateAccessoryContent()
    {
        if (CustomContent == null) return;

        var grid = RootGrid;
        if (grid == null) return;

        // Place custom content in accessory column
        Grid.SetColumn(CustomContent, 2);
        Grid.SetRowSpan(CustomContent, 2);
        CustomContent.VerticalOptions = LayoutOptions.Center;

        // Remove old accessory if present
        if (AccessoryView != null && grid.Children.Contains(AccessoryView))
            grid.Children.Remove(AccessoryView);

        grid.Children.Add(CustomContent);
    }

    private void UpdateArrowVisibility()
    {
        if (ShowArrow && _arrowLabel == null)
        {
            _arrowLabel = new Label
            {
                Text = "\u203A",
                FontSize = 20,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Colors.Gray
            };
        }

        if (_arrowLabel != null)
            _arrowLabel.IsVisible = ShowArrow;
    }

    protected override void OnTapped()
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }

    private void OnLongPressed(object? sender, TappedEventArgs e)
    {
        if (LongCommand?.CanExecute(LongCommandParameter) == true)
            LongCommand.Execute(LongCommandParameter);
    }
}
