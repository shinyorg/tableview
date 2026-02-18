using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.TableView.Cells;

public class DatePickerCell : CellBase
{
    private Label _valueLabel = default!;
    private DatePicker _hiddenPicker = default!;

    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime?), typeof(DatePickerCell), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((DatePickerCell)b).OnDateChanged());

    public static readonly BindableProperty InitialDateProperty = BindableProperty.Create(
        nameof(InitialDate), typeof(DateTime), typeof(DatePickerCell), new DateTime(2000, 1, 1));

    public static readonly BindableProperty MinimumDateProperty = BindableProperty.Create(
        nameof(MinimumDate), typeof(DateTime), typeof(DatePickerCell), new DateTime(1900, 1, 1),
        propertyChanged: (b, o, n) => ((DatePickerCell)b).SyncPickerRange());

    public static readonly BindableProperty MaximumDateProperty = BindableProperty.Create(
        nameof(MaximumDate), typeof(DateTime), typeof(DatePickerCell), new DateTime(2100, 12, 31),
        propertyChanged: (b, o, n) => ((DatePickerCell)b).SyncPickerRange());

    public static readonly BindableProperty FormatProperty = BindableProperty.Create(
        nameof(Format), typeof(string), typeof(DatePickerCell), "d",
        propertyChanged: (b, o, n) => ((DatePickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(DatePickerCell), null,
        propertyChanged: (b, o, n) => ((DatePickerCell)b).UpdateValueColor());

    public DateTime? Date
    {
        get => (DateTime?)GetValue(DateProperty);
        set => SetValue(DateProperty, value);
    }

    public DateTime InitialDate
    {
        get => (DateTime)GetValue(InitialDateProperty);
        set => SetValue(InitialDateProperty, value);
    }

    public DateTime MinimumDate
    {
        get => (DateTime)GetValue(MinimumDateProperty);
        set => SetValue(MinimumDateProperty, value);
    }

    public DateTime MaximumDate
    {
        get => (DateTime)GetValue(MaximumDateProperty);
        set => SetValue(MaximumDateProperty, value);
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
        var layout = new Grid();

        _valueLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        _hiddenPicker = new DatePicker
        {
            Opacity = 0,
            WidthRequest = 0,
            HeightRequest = 0,
            MinimumDate = MinimumDate,
            MaximumDate = MaximumDate
        };
        _hiddenPicker.DateSelected += (s, e) => Date = e.NewDate;

        layout.Children.Add(_hiddenPicker);
        layout.Children.Add(_valueLabel);

        UpdateDisplayText();
        return layout;
    }

    private void OnDateChanged()
    {
        if (_hiddenPicker != null && Date.HasValue)
            _hiddenPicker.Date = Date.Value;

        UpdateDisplayText();
    }

    private void SyncPickerRange()
    {
        if (_hiddenPicker == null) return;
        _hiddenPicker.MinimumDate = MinimumDate;
        _hiddenPicker.MaximumDate = MaximumDate;
    }

    private void UpdateDisplayText()
    {
        if (_valueLabel == null) return;
        _valueLabel.Text = Date?.ToString(Format) ?? string.Empty;
    }

    private void UpdateValueColor()
    {
        _valueLabel.TextColor = ResolveColor(ValueTextColor, ParentTableView?.CellValueTextColor, Colors.Gray);
    }

    protected override void OnTapped()
    {
        if (!Date.HasValue)
            _hiddenPicker.Date = InitialDate;

        _hiddenPicker.Focus();
    }
}
