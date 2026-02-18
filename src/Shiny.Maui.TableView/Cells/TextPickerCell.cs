using System.Collections;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace Shiny.Maui.TableView.Cells;

public class TextPickerCell : CellBase
{
    private Label _valueLabel = default!;
    private Picker _hiddenPicker = default!;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IList), typeof(TextPickerCell), null,
        propertyChanged: (b, o, n) => ((TextPickerCell)b).UpdatePickerItems());

    public static readonly BindableProperty SelectedIndexProperty = BindableProperty.Create(
        nameof(SelectedIndex), typeof(int), typeof(TextPickerCell), -1,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((TextPickerCell)b).OnSelectedIndexChanged());

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(TextPickerCell), null,
        BindingMode.TwoWay);

    public static readonly BindableProperty DisplayMemberProperty = BindableProperty.Create(
        nameof(DisplayMember), typeof(string), typeof(TextPickerCell), null);

    public static readonly BindableProperty PickerTitleProperty = BindableProperty.Create(
        nameof(PickerTitle), typeof(string), typeof(TextPickerCell), null,
        propertyChanged: (b, o, n) =>
        {
            var cell = (TextPickerCell)b;
            if (cell._hiddenPicker != null)
                cell._hiddenPicker.Title = (string?)n;
        });

    public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create(
        nameof(SelectedCommand), typeof(ICommand), typeof(TextPickerCell), null);

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(TextPickerCell), null,
        propertyChanged: (b, o, n) => ((TextPickerCell)b).UpdateValueColor());

    public IList? ItemsSource
    {
        get => (IList?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public string? DisplayMember
    {
        get => (string?)GetValue(DisplayMemberProperty);
        set => SetValue(DisplayMemberProperty, value);
    }

    public string? PickerTitle
    {
        get => (string?)GetValue(PickerTitleProperty);
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
        var layout = new Grid();

        _valueLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End
        };

        _hiddenPicker = new Picker
        {
            Opacity = 0,
            WidthRequest = 0,
            HeightRequest = 0,
            Title = PickerTitle
        };
        _hiddenPicker.SelectedIndexChanged += (s, e) =>
        {
            SelectedIndex = _hiddenPicker.SelectedIndex;
            SelectedItem = _hiddenPicker.SelectedItem;
            UpdateDisplayText();

            if (SelectedCommand?.CanExecute(SelectedItem) == true)
                SelectedCommand.Execute(SelectedItem);
        };

        layout.Children.Add(_hiddenPicker);
        layout.Children.Add(_valueLabel);

        return layout;
    }

    private void UpdatePickerItems()
    {
        if (_hiddenPicker == null || ItemsSource == null) return;

        _hiddenPicker.ItemsSource = ItemsSource;
        if (!string.IsNullOrEmpty(DisplayMember))
            _hiddenPicker.ItemDisplayBinding = new Binding(DisplayMember);
    }

    private void OnSelectedIndexChanged()
    {
        if (_hiddenPicker != null && _hiddenPicker.SelectedIndex != SelectedIndex)
            _hiddenPicker.SelectedIndex = SelectedIndex;
        UpdateDisplayText();
    }

    private void UpdateDisplayText()
    {
        if (_valueLabel == null || _hiddenPicker == null) return;

        if (_hiddenPicker.SelectedItem != null)
        {
            if (!string.IsNullOrEmpty(DisplayMember))
            {
                var prop = _hiddenPicker.SelectedItem.GetType().GetProperty(DisplayMember);
                _valueLabel.Text = prop?.GetValue(_hiddenPicker.SelectedItem)?.ToString() ?? _hiddenPicker.SelectedItem.ToString();
            }
            else
            {
                _valueLabel.Text = _hiddenPicker.SelectedItem.ToString();
            }
        }
        else
        {
            _valueLabel.Text = string.Empty;
        }
    }

    private void UpdateValueColor()
    {
        _valueLabel.TextColor = ResolveColor(ValueTextColor, ParentTableView?.CellValueTextColor, Colors.Gray);
    }

    protected override void OnTapped()
    {
        _hiddenPicker.Focus();
    }
}
