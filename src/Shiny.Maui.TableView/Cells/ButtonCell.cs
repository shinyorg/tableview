using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

public class ButtonCell : CellBase
{
    private Label _buttonLabel = default!;

    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(ButtonCell), null);

    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
        nameof(CommandParameter), typeof(object), typeof(ButtonCell), null);

    public static readonly BindableProperty ButtonTextColorProperty = BindableProperty.Create(
        nameof(ButtonTextColor), typeof(Color), typeof(ButtonCell), null,
        propertyChanged: (b, o, n) => ((ButtonCell)b).UpdateButtonColor());

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

    public Color? ButtonTextColor
    {
        get => (Color?)GetValue(ButtonTextColorProperty);
        set => SetValue(ButtonTextColorProperty, value);
    }

    public ButtonCell()
    {
        // ButtonCell uses a different layout - full-width centered text
        BuildButtonLayout();
    }

    private void BuildButtonLayout()
    {
        _buttonLabel = new Label
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            FontAttributes = FontAttributes.Bold,
            Padding = new Thickness(16, 12)
        };
        _buttonLabel.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        Content = _buttonLabel;
    }

    private void UpdateButtonColor()
    {
        _buttonLabel.TextColor = ButtonTextColor ?? ParentTableView?.CellAccentColor ?? Colors.Blue;
    }

    protected override void OnTapped()
    {
        if (Command?.CanExecute(CommandParameter) == true)
            Command.Execute(CommandParameter);
    }
}
