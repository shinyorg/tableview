using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Cells;

public class EntryCell : CellBase
{
    private Entry _entry = default!;

    public static readonly BindableProperty ValueTextProperty = BindableProperty.Create(
        nameof(ValueText), typeof(string), typeof(EntryCell), string.Empty,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((EntryCell)b).OnValueTextChanged((string)n));

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(EntryCell), null,
        propertyChanged: (b, o, n) => ((EntryCell)b).UpdateEntryColor());

    public static readonly BindableProperty ValueTextFontSizeProperty = BindableProperty.Create(
        nameof(ValueTextFontSize), typeof(double), typeof(EntryCell), -1d,
        propertyChanged: (b, o, n) => ((EntryCell)b).UpdateEntryFontSize());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
        nameof(Placeholder), typeof(string), typeof(EntryCell), string.Empty,
        propertyChanged: (b, o, n) => ((EntryCell)b)._entry.Placeholder = (string)n);

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard), typeof(Keyboard), typeof(EntryCell), Keyboard.Default,
        propertyChanged: (b, o, n) => ((EntryCell)b)._entry.Keyboard = (Keyboard)n);

    public static readonly BindableProperty IsPasswordProperty = BindableProperty.Create(
        nameof(IsPassword), typeof(bool), typeof(EntryCell), false,
        propertyChanged: (b, o, n) => ((EntryCell)b)._entry.IsPassword = (bool)n);

    public static readonly BindableProperty CompletedCommandProperty = BindableProperty.Create(
        nameof(CompletedCommand), typeof(ICommand), typeof(EntryCell), null);

    public string ValueText
    {
        get => (string)GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    public Color? ValueTextColor
    {
        get => (Color?)GetValue(ValueTextColorProperty);
        set => SetValue(ValueTextColorProperty, value);
    }

    public double ValueTextFontSize
    {
        get => (double)GetValue(ValueTextFontSizeProperty);
        set => SetValue(ValueTextFontSizeProperty, value);
    }

    public string Placeholder
    {
        get => (string)GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public Keyboard Keyboard
    {
        get => (Keyboard)GetValue(KeyboardProperty);
        set => SetValue(KeyboardProperty, value);
    }

    public bool IsPassword
    {
        get => (bool)GetValue(IsPasswordProperty);
        set => SetValue(IsPasswordProperty, value);
    }

    public ICommand? CompletedCommand
    {
        get => (ICommand?)GetValue(CompletedCommandProperty);
        set => SetValue(CompletedCommandProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        _entry = new Entry
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Fill,
            HorizontalTextAlignment = TextAlignment.End,
            MinimumWidthRequest = 120
        };
        _entry.TextChanged += (s, e) =>
        {
            if (ValueText != e.NewTextValue)
                ValueText = e.NewTextValue;
        };
        _entry.Completed += (s, e) =>
        {
            if (CompletedCommand?.CanExecute(ValueText) == true)
                CompletedCommand.Execute(ValueText);
        };
        return _entry;
    }

    private void OnValueTextChanged(string newValue)
    {
        if (_entry != null && _entry.Text != newValue)
            _entry.Text = newValue;
    }

    private void UpdateEntryColor()
    {
        _entry.TextColor = ResolveColor(ValueTextColor, ParentTableView?.CellValueTextColor, Colors.Black);
    }

    private void UpdateEntryFontSize()
    {
        _entry.FontSize = ResolveDouble(ValueTextFontSize, ParentTableView?.CellValueTextFontSize ?? -1, 16);
    }

    protected override void OnTapped()
    {
        _entry.Focus();
    }
}
