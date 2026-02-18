using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

public class CheckboxCell : CellBase
{
    private CheckBox _checkBox = default!;

    public static readonly BindableProperty CheckedProperty = BindableProperty.Create(
        nameof(Checked), typeof(bool), typeof(CheckboxCell), false,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((CheckboxCell)b)._checkBox.IsChecked = (bool)n);

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor), typeof(Color), typeof(CheckboxCell), null,
        propertyChanged: (b, o, n) => ((CheckboxCell)b).UpdateCheckBoxColor());

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
        _checkBox = new CheckBox
        {
            VerticalOptions = LayoutOptions.Center
        };
        _checkBox.CheckedChanged += (s, e) => Checked = e.Value;
        return _checkBox;
    }

    private void UpdateCheckBoxColor()
    {
        var color = AccentColor ?? ParentTableView?.CellAccentColor;
        if (color != null)
            _checkBox.Color = color;
    }

    protected override void OnTapped()
    {
        _checkBox.IsChecked = !_checkBox.IsChecked;
    }
}
