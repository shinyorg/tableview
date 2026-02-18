using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.TableView.Cells;

public class TimePickerCell : CellBase
{
    private Label _valueLabel = default!;
    private TimePicker _hiddenPicker = default!;

    public static readonly BindableProperty TimeProperty = BindableProperty.Create(
        nameof(Time), typeof(TimeSpan), typeof(TimePickerCell), TimeSpan.Zero,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((TimePickerCell)b).OnTimeChanged());

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(TimePickerCell), "t",
        propertyChanged: (b, o, n) => ((TimePickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(TimePickerCell), null,
        propertyChanged: (b, o, n) => ((TimePickerCell)b).UpdateValueColor());

    public TimeSpan Time
    {
        get => (TimeSpan)GetValue(TimeProperty);
        set => SetValue(TimeProperty, value);
    }

    public string Format
    {
        get => (string)GetValue(FormatProperty);
        set => SetValue(FormatProperty, value);
    }

    public Color? ValueTextColor
    {
        get => (Color?)GetValue(ValueTextColorProperty);
        set => SetValue(ValueTextColorProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        _valueLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        _hiddenPicker = new TimePicker
        {
            Opacity = 0.01
        };
        _hiddenPicker.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(TimePicker.Time))
                Time = _hiddenPicker.Time ?? TimeSpan.Zero;
        };
        _hiddenPicker.Focused += (s, e) => ApplySelectionHighlight();
        _hiddenPicker.Unfocused += (s, e) => ClearSelectionHighlight();

        // Overlay the transparent picker across the entire cell so tapping
        // anywhere opens the native time dialog (Focus() is unreliable on Android)
        Grid.SetColumn(_hiddenPicker, 0);
        Grid.SetColumnSpan(_hiddenPicker, 3);
        Grid.SetRow(_hiddenPicker, 0);
        Grid.SetRowSpan(_hiddenPicker, 2);
        RootGrid.Children.Add(_hiddenPicker);

        UpdateDisplayText();
        return _valueLabel;
    }

    protected override void OnCellTapped(object? sender, TappedEventArgs e)
    {
        // Native picker overlay handles all touch interaction
    }

    private void OnTimeChanged()
    {
        if (_hiddenPicker != null)
            _hiddenPicker.Time = Time;
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        if (_valueLabel == null) return;
        var dt = DateTime.Today.Add(Time);
        _valueLabel.Text = dt.ToString(Format);
    }

    private void UpdateValueColor()
    {
        var color = ValueTextColor ?? ParentTableView?.CellValueTextColor;
        if (color != null)
            _valueLabel.TextColor = color;
        else
            _valueLabel.ClearValue(Label.TextColorProperty);
    }
}
