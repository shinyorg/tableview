using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.TableView.Cells;

namespace Shiny.Maui.TableView.Pages;

public class PickerPage : ContentPage
{
    private readonly PickerCell _ownerCell;
    private readonly ObservableCollection<PickerItemViewModel> _items = new();
    private readonly CollectionView _collectionView;

    public PickerPage(PickerCell ownerCell)
    {
        _ownerCell = ownerCell;
        Title = ownerCell.PageTitle;

        var accentColor = ownerCell.AccentColor ?? ownerCell.ParentTableView?.CellAccentColor ?? Colors.Blue;

        _collectionView = new CollectionView
        {
            ItemsSource = _items,
            SelectionMode = Microsoft.Maui.Controls.SelectionMode.None,
            ItemTemplate = new DataTemplate(() =>
            {
                var grid = new Grid
                {
                    Padding = new Thickness(16, 12),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition(GridLength.Star),
                        new ColumnDefinition(GridLength.Auto)
                    },
                    RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(GridLength.Auto)
                    }
                };

                var displayLabel = new Label
                {
                    FontSize = 16,
                    VerticalOptions = LayoutOptions.Center
                };
                displayLabel.SetBinding(Label.TextProperty, nameof(PickerItemViewModel.DisplayText));
                Grid.SetColumn(displayLabel, 0);
                grid.Children.Add(displayLabel);

                var subLabel = new Label
                {
                    FontSize = 12,
                    Opacity = 0.6,
                    VerticalOptions = LayoutOptions.Start
                };
                subLabel.SetBinding(Label.TextProperty, nameof(PickerItemViewModel.SubDisplayText));
                subLabel.SetBinding(Label.IsVisibleProperty, nameof(PickerItemViewModel.HasSubText));
                Grid.SetColumn(subLabel, 0);
                Grid.SetRow(subLabel, 1);
                grid.Children.Add(subLabel);

                var checkLabel = new Label
                {
                    Text = "\u2713",
                    FontSize = 20,
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = accentColor
                };
                checkLabel.SetBinding(Label.IsVisibleProperty, nameof(PickerItemViewModel.IsSelected));
                Grid.SetColumn(checkLabel, 1);
                Grid.SetRowSpan(checkLabel, 2);
                grid.Children.Add(checkLabel);

                var tapGesture = new TapGestureRecognizer();
                tapGesture.Tapped += OnItemTapped;
                grid.GestureRecognizers.Add(tapGesture);

                return grid;
            })
        };

        Content = _collectionView;
        LoadItems();
    }

    private void LoadItems()
    {
        if (_ownerCell.ItemsSource == null) return;

        var selectedItems = _ownerCell.SelectedItems;
        var selectedItem = _ownerCell.SelectedItem;

        foreach (var item in _ownerCell.ItemsSource)
        {
            var isSelected = false;
            if (_ownerCell.SelectionMode == Cells.SelectionMode.Single)
                isSelected = Equals(item, selectedItem);
            else if (selectedItems != null)
                isSelected = selectedItems.Contains(item);

            _items.Add(new PickerItemViewModel
            {
                Item = item,
                DisplayText = _ownerCell.GetDisplayText(item),
                SubDisplayText = _ownerCell.GetSubDisplayText(item),
                IsSelected = isSelected
            });
        }
    }

    private async void OnItemTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not View view || view.BindingContext is not PickerItemViewModel vm)
            return;

        if (_ownerCell.SelectionMode == Cells.SelectionMode.Single)
        {
            foreach (var item in _items)
                item.IsSelected = false;
            vm.IsSelected = true;

            _ownerCell.OnSelectionComplete(vm.Item, null);
            await Navigation.PopAsync();
        }
        else
        {
            if (vm.IsSelected)
            {
                vm.IsSelected = false;
            }
            else
            {
                var currentCount = _items.Count(i => i.IsSelected);
                if (_ownerCell.MaxSelectedNumber > 0 && currentCount >= _ownerCell.MaxSelectedNumber)
                    return;

                vm.IsSelected = true;
            }

            var selected = new ObservableCollection<object>();
            foreach (var item in _items.Where(i => i.IsSelected))
                selected.Add(item.Item);

            _ownerCell.OnSelectionComplete(null, selected);

            if (_ownerCell.UsePickToClose &&
                _ownerCell.MaxSelectedNumber > 0 &&
                selected.Count >= _ownerCell.MaxSelectedNumber)
            {
                await Navigation.PopAsync();
            }
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (_ownerCell.SelectionMode == Cells.SelectionMode.Multiple)
        {
            var selected = new ObservableCollection<object>();
            foreach (var item in _items.Where(i => i.IsSelected))
                selected.Add(item.Item);
            _ownerCell.OnSelectionComplete(null, selected);
        }
    }
}

internal class PickerItemViewModel : BindableObject
{
    public object Item { get; set; } = default!;

    public static readonly BindableProperty DisplayTextProperty = BindableProperty.Create(
        nameof(DisplayText), typeof(string), typeof(PickerItemViewModel), string.Empty);

    public static readonly BindableProperty SubDisplayTextProperty = BindableProperty.Create(
        nameof(SubDisplayText), typeof(string), typeof(PickerItemViewModel), string.Empty);

    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
        nameof(IsSelected), typeof(bool), typeof(PickerItemViewModel), false);

    public static readonly BindableProperty HasSubTextProperty = BindableProperty.Create(
        nameof(HasSubText), typeof(bool), typeof(PickerItemViewModel), false);

    public string DisplayText
    {
        get => (string)GetValue(DisplayTextProperty);
        set
        {
            SetValue(DisplayTextProperty, value);
            OnPropertyChanged(nameof(DisplayText));
        }
    }

    public string SubDisplayText
    {
        get => (string)GetValue(SubDisplayTextProperty);
        set
        {
            SetValue(SubDisplayTextProperty, value);
            HasSubText = !string.IsNullOrEmpty(value);
            OnPropertyChanged(nameof(SubDisplayText));
        }
    }

    public bool IsSelected
    {
        get => (bool)GetValue(IsSelectedProperty);
        set
        {
            SetValue(IsSelectedProperty, value);
            OnPropertyChanged(nameof(IsSelected));
        }
    }

    public bool HasSubText
    {
        get => (bool)GetValue(HasSubTextProperty);
        set
        {
            SetValue(HasSubTextProperty, value);
            OnPropertyChanged(nameof(HasSubText));
        }
    }
}
