using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;
using TvTableSection = Shiny.Maui.TableView.Sections.TableSection;

namespace Shiny.Maui.TableView.Cells;

public class RadioCell : CellBase
{
    private RadioButton _radioButton = default!;

    public static readonly BindableProperty ValueProperty = BindableProperty.Create(
        nameof(Value), typeof(object), typeof(RadioCell), null);

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor), typeof(Color), typeof(RadioCell), null);

    // Attached property for section-level selected value
    public static readonly BindableProperty SelectedValueProperty = BindableProperty.CreateAttached(
        "SelectedValue", typeof(object), typeof(RadioCell), null,
        BindingMode.TwoWay,
        propertyChanged: OnSelectedValueChanged);

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Color? AccentColor
    {
        get => (Color?)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public static object? GetSelectedValue(BindableObject obj) => obj.GetValue(SelectedValueProperty);
    public static void SetSelectedValue(BindableObject obj, object? value) => obj.SetValue(SelectedValueProperty, value);

    protected override View? CreateAccessoryView()
    {
        _radioButton = new RadioButton
        {
            VerticalOptions = LayoutOptions.Center
        };
        _radioButton.CheckedChanged += OnRadioCheckedChanged;
        return _radioButton;
    }

    private void OnRadioCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value && ParentSection != null)
        {
            SetSelectedValue(ParentSection, Value);
        }
    }

    private static void OnSelectedValueChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is TvTableSection section)
        {
            foreach (var cell in section.GetVisibleCells())
            {
                if (cell is RadioCell radioCell)
                {
                    radioCell._radioButton.IsChecked = Equals(radioCell.Value, newValue);
                }
            }
        }
    }

    protected override void OnTapped()
    {
        _radioButton.IsChecked = true;
    }
}
