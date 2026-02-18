using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.TableView.Pages;

namespace Shiny.Maui.TableView.Cells;

public enum SelectionMode
{
    Single,
    Multiple
}

public class PickerCell : CellBase
{
    private Label _valueLabel = default!;
    private Label _arrowLabel = default!;

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable), typeof(PickerCell), null);

    public static readonly BindableProperty SelectedItemsProperty = BindableProperty.Create(
        nameof(SelectedItems), typeof(IList), typeof(PickerCell), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((PickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(
        nameof(SelectedItem), typeof(object), typeof(PickerCell), null,
        BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((PickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty SelectionModeProperty = BindableProperty.Create(
        nameof(SelectionMode), typeof(SelectionMode), typeof(PickerCell), SelectionMode.Single);

    public static readonly BindableProperty MaxSelectedNumberProperty = BindableProperty.Create(
        nameof(MaxSelectedNumber), typeof(int), typeof(PickerCell), 0);

    public static readonly BindableProperty UsePickToCloseProperty = BindableProperty.Create(
        nameof(UsePickToClose), typeof(bool), typeof(PickerCell), false);

    public static readonly BindableProperty UseAutoValueTextProperty = BindableProperty.Create(
        nameof(UseAutoValueText), typeof(bool), typeof(PickerCell), true,
        propertyChanged: (b, o, n) => ((PickerCell)b).UpdateDisplayText());

    public static readonly BindableProperty DisplayMemberProperty = BindableProperty.Create(
        nameof(DisplayMember), typeof(string), typeof(PickerCell), null);

    public static readonly BindableProperty SubDisplayMemberProperty = BindableProperty.Create(
        nameof(SubDisplayMember), typeof(string), typeof(PickerCell), null);

    public static readonly BindableProperty PageTitleProperty = BindableProperty.Create(
        nameof(PageTitle), typeof(string), typeof(PickerCell), "Select");

    public static readonly BindableProperty SelectedCommandProperty = BindableProperty.Create(
        nameof(SelectedCommand), typeof(ICommand), typeof(PickerCell), null);

    public static readonly BindableProperty ValueTextColorProperty = BindableProperty.Create(
        nameof(ValueTextColor), typeof(Color), typeof(PickerCell), null,
        propertyChanged: (b, o, n) => ((PickerCell)b).UpdateValueColor());

    public static readonly BindableProperty AccentColorProperty = BindableProperty.Create(
        nameof(AccentColor), typeof(Color), typeof(PickerCell), null);

    public static readonly BindableProperty KeepSelectedUntilBackProperty = BindableProperty.Create(
        nameof(KeepSelectedUntilBack), typeof(bool), typeof(PickerCell), false);

    public static readonly BindableProperty ShowArrowProperty = BindableProperty.Create(
        nameof(ShowArrow), typeof(bool), typeof(PickerCell), true,
        propertyChanged: (b, o, n) => ((PickerCell)b)._arrowLabel.IsVisible = (bool)n);

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IList? SelectedItems
    {
        get => (IList?)GetValue(SelectedItemsProperty);
        set => SetValue(SelectedItemsProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public SelectionMode SelectionMode
    {
        get => (SelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public int MaxSelectedNumber
    {
        get => (int)GetValue(MaxSelectedNumberProperty);
        set => SetValue(MaxSelectedNumberProperty, value);
    }

    public bool UsePickToClose
    {
        get => (bool)GetValue(UsePickToCloseProperty);
        set => SetValue(UsePickToCloseProperty, value);
    }

    public bool UseAutoValueText
    {
        get => (bool)GetValue(UseAutoValueTextProperty);
        set => SetValue(UseAutoValueTextProperty, value);
    }

    public string? DisplayMember
    {
        get => (string?)GetValue(DisplayMemberProperty);
        set => SetValue(DisplayMemberProperty, value);
    }

    public string? SubDisplayMember
    {
        get => (string?)GetValue(SubDisplayMemberProperty);
        set => SetValue(SubDisplayMemberProperty, value);
    }

    public string PageTitle
    {
        get => (string)GetValue(PageTitleProperty);
        set => SetValue(PageTitleProperty, value);
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

    public Color? AccentColor
    {
        get => (Color?)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    public bool KeepSelectedUntilBack
    {
        get => (bool)GetValue(KeepSelectedUntilBackProperty);
        set => SetValue(KeepSelectedUntilBackProperty, value);
    }

    public bool ShowArrow
    {
        get => (bool)GetValue(ShowArrowProperty);
        set => SetValue(ShowArrowProperty, value);
    }

    protected override View? CreateAccessoryView()
    {
        var layout = new HorizontalStackLayout
        {
            VerticalOptions = LayoutOptions.Center,
            Spacing = 4
        };

        _valueLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            LineBreakMode = LineBreakMode.TailTruncation,
            MaximumWidthRequest = 200
        };

        _arrowLabel = new Label
        {
            Text = "\u203A",
            FontSize = 20,
            VerticalOptions = LayoutOptions.Center,
            TextColor = Colors.Gray
        };

        layout.Children.Add(_valueLabel);
        layout.Children.Add(_arrowLabel);

        return layout;
    }

    protected override bool ShouldKeepSelection() => KeepSelectedUntilBack;

    private void UpdateDisplayText()
    {
        if (_valueLabel == null || !UseAutoValueText) return;

        if (SelectionMode == SelectionMode.Single && SelectedItem != null)
        {
            _valueLabel.Text = GetDisplayText(SelectedItem);
        }
        else if (SelectionMode == SelectionMode.Multiple && SelectedItems != null)
        {
            var texts = new List<string>();
            foreach (var item in SelectedItems)
                texts.Add(GetDisplayText(item));
            _valueLabel.Text = string.Join(", ", texts);
        }
        else
        {
            _valueLabel.Text = string.Empty;
        }
    }

    internal string GetDisplayText(object item)
    {
        if (!string.IsNullOrEmpty(DisplayMember))
        {
            var prop = item.GetType().GetProperty(DisplayMember);
            return prop?.GetValue(item)?.ToString() ?? item.ToString() ?? string.Empty;
        }
        return item.ToString() ?? string.Empty;
    }

    internal string GetSubDisplayText(object item)
    {
        if (!string.IsNullOrEmpty(SubDisplayMember))
        {
            var prop = item.GetType().GetProperty(SubDisplayMember);
            return prop?.GetValue(item)?.ToString() ?? string.Empty;
        }
        return string.Empty;
    }

    private void UpdateValueColor()
    {
        _valueLabel.TextColor = ResolveColor(ValueTextColor, ParentTableView?.CellValueTextColor, Colors.Gray);
    }

    protected override async void OnTapped()
    {
        var page = GetParentPage();
        if (page?.Navigation == null || ItemsSource == null) return;

        if (KeepSelectedUntilBack)
        {
            void handler(object? s, EventArgs args)
            {
                ClearSelectionHighlight();
                page.Appearing -= handler;
            }
            page.Appearing += handler;
        }

        var pickerPage = new PickerPage(this);
        await page.Navigation.PushAsync(pickerPage);
    }

    internal void OnSelectionComplete(object? selectedItem, IList? selectedItems)
    {
        if (SelectionMode == SelectionMode.Single)
            SelectedItem = selectedItem;
        else
            SelectedItems = selectedItems;

        UpdateDisplayText();

        if (SelectedCommand?.CanExecute(null) == true)
            SelectedCommand.Execute(SelectionMode == SelectionMode.Single ? SelectedItem : SelectedItems);
    }
}
