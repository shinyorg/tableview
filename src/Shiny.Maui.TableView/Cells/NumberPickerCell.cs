using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

public class NumberPickerCell : CellBase
{
    private Label _valueLabel = default!;

    public static readonly BindableProperty NumberProperty = BindableProperty.Create(
        nameof(Number), typeof(int), typeof(NumberPickerCell), 0,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((NumberPickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty MinProperty = BindableProperty.Create(
        nameof(Min), typeof(int), typeof(NumberPickerCell), 0);

    public static readonly BindableProperty MaxProperty = BindableProperty.Create(
        nameof(Max), typeof(int), typeof(NumberPickerCell), 100);

    public static readonly BindableProperty UnitProperty = BindableProperty.Create(
        nameof(Unit), typeof(string), typeof(NumberPickerCell), string.Empty,
        propertyChanged: (b, o, n) => ((NumberPickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty PickerTitleProperty = BindableProperty.Create(
        nameof(PickerTitle), typeof(string), typeof(NumberPickerCell), "Enter a number");

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(NumberPickerCell), null,
        propertyChanged: (b, o, n) => ((NumberPickerCell)b).UpdateValueColor());

    public int Number
    {
        get => (int)GetValue(NumberProperty);
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
        _valueLabel.Text = string.IsNullOrEmpty(Unit) ? Number.ToString() : $"{Number} {Unit}";
    }

    private void UpdateValueColor()
    {
        _valueLabel.TextColor = ResolveColor(ValueTextColor, ParentTableView?.CellValueTextColor, Colors.Gray);
    }

    protected override async void OnTapped()
    {
        var page = GetParentPage();
        if (page == null) return;

        var result = await page.DisplayPromptAsync(
            PickerTitle,
            $"Enter a number between {Min} and {Max}",
            keyboard: Keyboard.Numeric,
            initialValue: Number.ToString());

        if (result != null && int.TryParse(result, out var value))
        {
            value = Math.Clamp(value, Min, Max);
            Number = value;
        }
    }

    private Page? GetParentPage()
    {
        Element? current = this;
        while (current != null)
        {
            if (current is Page page)
                return page;
            current = current.Parent;
        }
        return null;
    }
}
