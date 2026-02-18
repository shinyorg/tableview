using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

public class SwitchCell : CellBase
{
    private Switch _switch = default!;

    public static readonly BindableProperty OnProperty = BindableProperty.Create(
        nameof(On), typeof(bool), typeof(SwitchCell), false,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((SwitchCell)b)._switch.IsToggled = (bool)n);

    public static readonly BindableProperty OnColorProperty = BindableProperty.Create(
        nameof(OnColor), typeof(Color), typeof(SwitchCell), null,
        propertyChanged: (b, o, n) => ((SwitchCell)b).UpdateSwitchColor());

    public bool On
    {
        get => (bool)GetValue(OnProperty);
        set => SetValue(OnProperty, value);
    }

    public Color? OnColor
    {
        get => (Color?)GetValue(OnColorProperty);
        set => SetValue(OnColorProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        _switch = new Switch
        {
            VerticalOptions = LayoutOptions.Center
        };
        _switch.Toggled += (s, e) => On = e.Value;
        return _switch;
    }

    private void UpdateSwitchColor()
    {
        var color = OnColor ?? ParentTableView?.CellAccentColor;
        if (color != null)
            _switch.OnColor = color;
    }

    protected override void OnTapped()
    {
        _switch.IsToggled = !_switch.IsToggled;
    }
}
