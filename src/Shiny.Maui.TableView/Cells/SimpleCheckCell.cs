using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

public class SimpleCheckCell : CellBase
{
    private Label _checkLabel = default!;

    public static readonly BindableProperty CheckedProperty = BindableProperty.Create(
        nameof(Checked), typeof(bool), typeof(SimpleCheckCell), false,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((SimpleCheckCell)b).UpdateCheckVisibility());

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor), typeof(Color), typeof(SimpleCheckCell), null,
        propertyChanged: (b, o, n) => ((SimpleCheckCell)b).UpdateCheckColor());

    public bool Checked
    {
        get => (bool)GetValue(CheckedProperty);
        set => SetValue(CheckedProperty, value);
    }

    public Color? AccentColor
    {
        get => (Color?)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        _checkLabel = new Label
        {
            Text = "\u2713",
            FontSize = 20,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
            IsVisible = false
        };
        return _checkLabel;
    }

    private void UpdateCheckVisibility()
    {
        _checkLabel.IsVisible = Checked;
    }

    private void UpdateCheckColor()
    {
        _checkLabel.TextColor = AccentColor ?? ParentTableView?.CellAccentColor ?? Colors.Blue;
    }

    protected override void OnTapped()
    {
        Checked = !Checked;
    }
}
