using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.TableView.Cells;

public class NumberPickerCell : CellBase
{
    private Label _valueLabel = default!;

    public static readonly BindableProperty NumberProperty = BindableProperty.Create(
        nameof(Number), typeof(int?), typeof(NumberPickerCell), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((NumberPickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty MinProperty = BindableProperty.Create(
        nameof(Min), typeof(int), typeof(NumberPickerCell), 0);

    public static readonly BindableProperty MaxProperty = BindableProperty.Create(
        nameof(Max), typeof(int), typeof(NumberPickerCell), 9999);

    public static readonly BindableProperty UnitProperty = BindableProperty.Create(
        nameof(Unit), typeof(string), typeof(NumberPickerCell), string.Empty,
        propertyChanged: (b, o, n) => ((NumberPickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty PickerTitleProperty = BindableProperty.Create(
        nameof(PickerTitle), typeof(string), typeof(NumberPickerCell), "Enter a number");

    public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create(
        nameof(SelectedCommand), typeof(ICommand), typeof(NumberPickerCell), null);

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(NumberPickerCell), null,
        propertyChanged: (b, o, n) => ((NumberPickerCell)b).UpdateValueColor());

    public int? Number
    {
        get => (int?)GetValue(NumberProperty);
        set => SetValue(NumberProperty, value);
    }

    public int Min
    {
        get => (int)GetValue(MinProperty);
        set => SetValue(MinProperty, value);
    }

    public int Max
    {
        get => (int)GetValue(MaxProperty);
        set => SetValue(MaxProperty, value);
    }

    public string Unit
    {
        get => (string)GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
    }

    public string PickerTitle
    {
        get => (string)GetValue(PickerTitleProperty);
        set => SetValue(PickerTitleProperty, value);
    }

    public ICommand? SelectedCommand
    {
        get => (ICommand?)GetValue(SelectedCommandProperty);
        set => SetValue(SelectedCommandProperty, value);
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
        UpdateDisplayText();
        return _valueLabel;
    }

    private void UpdateDisplayText()
    {
        if (_valueLabel == null) return;

        if (Number.HasValue)
            _valueLabel.Text = string.IsNullOrEmpty(Unit) ? Number.Value.ToString() : $"{Number.Value} {Unit}";
        else
            _valueLabel.Text = string.Empty;
    }

    private void UpdateValueColor()
    {
        var color = ValueTextColor ?? ParentTableView?.CellValueTextColor;
        if (color != null)
            _valueLabel.TextColor = color;
        else
            _valueLabel.ClearValue(Label.TextColorProperty);
    }

    protected override bool ShouldKeepSelection() => true;

    protected override async void OnTapped()
    {
        var page = GetParentPage();
        if (page == null) return;

        var result = await page.DisplayPromptAsync(
            PickerTitle,
            $"Enter a number between {Min} and {Max}",
            keyboard: Keyboard.Numeric,
            initialValue: Number?.ToString() ?? string.Empty);

        ClearSelectionHighlight();

        if (result != null && int.TryParse(result, out var value))
        {
            value = Math.Clamp(value, Min, Max);
            Number = value;

            if (SelectedCommand?.CanExecute(Number) == true)
                SelectedCommand.Execute(Number);
        }
    }
}
