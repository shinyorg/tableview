using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.TableView.Cells;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Sections;

[ContentProperty(nameof(Cells))]
public class TableSection : BindableObject
{
    private readonly ObservableCollection<CellBase> _cells = new();
    private INotifyCollectionChanged? _itemsSourceNotifier;
    private readonly List<CellBase> _generatedCells = new();

    public TableSection()
    {
        _cells.CollectionChanged += OnCellsCollectionChanged;
    }

    public TableSection(string title) : this()
    {
        Title = title;
    }

    #region Bindable Properties

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(TableSection), string.Empty,
        propertyChanged: (b, o, n) => ((TableSection)b).RaiseSectionChanged());

    public static readonly BindableProperty FooterTextProperty = BindableProperty.Create(
        nameof(FooterText), typeof(string), typeof(TableSection), string.Empty,
        propertyChanged: (b, o, n) => ((TableSection)b).RaiseSectionChanged());

    public static readonly BindableProperty HeaderViewProperty = BindableProperty.Create(
        nameof(HeaderView), typeof(View), typeof(TableSection), null,
        propertyChanged: (b, o, n) => ((TableSection)b).RaiseSectionChanged());

    public static readonly BindableProperty FooterViewProperty = BindableProperty.Create(
        nameof(FooterView), typeof(View), typeof(TableSection), null,
        propertyChanged: (b, o, n) => ((TableSection)b).RaiseSectionChanged());

    public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(
        nameof(IsVisible), typeof(bool), typeof(TableSection), true,
        propertyChanged: (b, o, n) => ((TableSection)b).RaiseSectionChanged());

    public static readonly BindableProperty FooterVisibleProperty = BindableProperty.Create(
        nameof(FooterVisible), typeof(bool), typeof(TableSection), true,
        propertyChanged: (b, o, n) => ((TableSection)b).RaiseSectionChanged());

    public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(
        nameof(HeaderBackgroundColor), typeof(Color), typeof(TableSection), null);

    public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create(
        nameof(HeaderTextColor), typeof(Color), typeof(TableSection), null);

    public static readonly BindableProperty HeaderFontSizeProperty = BindableProperty.Create(
        nameof(HeaderFontSize), typeof(double), typeof(TableSection), -1d);

    public static readonly BindableProperty HeaderFontFamilyProperty = BindableProperty.Create(
        nameof(HeaderFontFamily), typeof(string), typeof(TableSection), null);

    public static readonly BindableProperty HeaderFontAttributesProperty = BindableProperty.Create(
        nameof(HeaderFontAttributes), typeof(FontAttributes?), typeof(TableSection), null);

    public static readonly BindableProperty HeaderHeightProperty = BindableProperty.Create(
        nameof(HeaderHeight), typeof(double), typeof(TableSection), -1d);

    public static readonly BindableProperty FooterTextColorProperty = BindableProperty.Create(
        nameof(FooterTextColor), typeof(Color), typeof(TableSection), null);

    public static readonly BindableProperty FooterFontSizeProperty = BindableProperty.Create(
        nameof(FooterFontSize), typeof(double), typeof(TableSection), -1d);

    public static readonly BindableProperty FooterFontFamilyProperty = BindableProperty.Create(
        nameof(FooterFontFamily), typeof(string), typeof(TableSection), null);

    public static readonly BindableProperty FooterFontAttributesProperty = BindableProperty.Create(
        nameof(FooterFontAttributes), typeof(FontAttributes?), typeof(TableSection), null);

    public static readonly BindableProperty FooterBackgroundColorProperty = BindableProperty.Create(
        nameof(FooterBackgroundColor), typeof(Color), typeof(TableSection), null);

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable), typeof(TableSection), null,
        propertyChanged: OnItemsSourceChanged);

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TableSection), null,
        propertyChanged: (b, o, n) => ((TableSection)b).RegenerateTemplatedCells());

    public static readonly BindableProperty TemplateStartIndexProperty = BindableProperty.Create(
        nameof(TemplateStartIndex), typeof(int), typeof(TableSection), 0,
        propertyChanged: (b, o, n) => ((TableSection)b).RegenerateTemplatedCells());

    public static readonly BindableProperty UseDragSortProperty = BindableProperty.Create(
        nameof(UseDragSort), typeof(bool), typeof(TableSection), false,
        propertyChanged: (b, o, n) => ((TableSection)b).RaiseSectionChanged());

    #endregion

    #region Properties

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string FooterText
    {
        get => (string)GetValue(FooterTextProperty);
        set => SetValue(FooterTextProperty, value);
    }

    public View? HeaderView
    {
        get => (View?)GetValue(HeaderViewProperty);
        set => SetValue(HeaderViewProperty, value);
    }

    public View? FooterView
    {
        get => (View?)GetValue(FooterViewProperty);
        set => SetValue(FooterViewProperty, value);
    }

    public bool IsVisible
    {
        get => (bool)GetValue(IsVisibleProperty);
        set => SetValue(IsVisibleProperty, value);
    }

    public bool FooterVisible
    {
        get => (bool)GetValue(FooterVisibleProperty);
        set => SetValue(FooterVisibleProperty, value);
    }

    public Color? HeaderBackgroundColor
    {
        get => (Color?)GetValue(HeaderBackgroundColorProperty);
        set => SetValue(HeaderBackgroundColorProperty, value);
    }

    public Color? HeaderTextColor
    {
        get => (Color?)GetValue(HeaderTextColorProperty);
        set => SetValue(HeaderTextColorProperty, value);
    }

    public double HeaderFontSize
    {
        get => (double)GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    public string? HeaderFontFamily
    {
        get => (string?)GetValue(HeaderFontFamilyProperty);
        set => SetValue(HeaderFontFamilyProperty, value);
    }

    public FontAttributes? HeaderFontAttributes
    {
        get => (FontAttributes?)GetValue(HeaderFontAttributesProperty);
        set => SetValue(HeaderFontAttributesProperty, value);
    }

    public double HeaderHeight
    {
        get => (double)GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    public Color? FooterTextColor
    {
        get => (Color?)GetValue(FooterTextColorProperty);
        set => SetValue(FooterTextColorProperty, value);
    }

    public double FooterFontSize
    {
        get => (double)GetValue(FooterFontSizeProperty);
        set => SetValue(FooterFontSizeProperty, value);
    }

    public string? FooterFontFamily
    {
        get => (string?)GetValue(FooterFontFamilyProperty);
        set => SetValue(FooterFontFamilyProperty, value);
    }

    public FontAttributes? FooterFontAttributes
    {
        get => (FontAttributes?)GetValue(FooterFontAttributesProperty);
        set => SetValue(FooterFontAttributesProperty, value);
    }

    public Color? FooterBackgroundColor
    {
        get => (Color?)GetValue(FooterBackgroundColorProperty);
        set => SetValue(FooterBackgroundColorProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => (IEnumerable?)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public int TemplateStartIndex
    {
        get => (int)GetValue(TemplateStartIndexProperty);
        set => SetValue(TemplateStartIndexProperty, value);
    }

    public bool UseDragSort
    {
        get => (bool)GetValue(UseDragSortProperty);
        set => SetValue(UseDragSortProperty, value);
    }

    public ObservableCollection<CellBase> Cells => _cells;

    internal TvTableView? ParentTableView { get; set; }

    #endregion

    #region Cell Collection

    public IReadOnlyList<CellBase> GetVisibleCells()
    {
        var all = GetAllCells();
        // Filter by cell IsVisible
        var visible = new List<CellBase>(all.Count);
        foreach (var cell in all)
        {
            if (cell.IsVisible)
                visible.Add(cell);
        }
        return visible;
    }

    internal IReadOnlyList<CellBase> GetAllCells()
    {
        if (_generatedCells.Count == 0)
            return _cells;

        var result = new List<CellBase>();
        var insertIndex = Math.Min(TemplateStartIndex, _cells.Count);

        for (int i = 0; i < insertIndex; i++)
            result.Add(_cells[i]);

        result.AddRange(_generatedCells);

        for (int i = insertIndex; i < _cells.Count; i++)
            result.Add(_cells[i]);

        return result;
    }

    private void OnCellsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RaiseSectionChanged();
    }

    #endregion

    #region ItemsSource

    private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var section = (TableSection)bindable;

        if (section._itemsSourceNotifier != null)
        {
            section._itemsSourceNotifier.CollectionChanged -= section.OnItemsSourceCollectionChanged;
            section._itemsSourceNotifier = null;
        }

        if (newValue is INotifyCollectionChanged notifier)
        {
            section._itemsSourceNotifier = notifier;
            notifier.CollectionChanged += section.OnItemsSourceCollectionChanged;
        }

        section.RegenerateTemplatedCells();
    }

    private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RegenerateTemplatedCells();
    }

    internal void RegenerateTemplatedCells()
    {
        _generatedCells.Clear();

        if (ItemsSource == null || ItemTemplate == null)
        {
            RaiseSectionChanged();
            return;
        }

        foreach (var item in ItemsSource)
        {
            var template = ItemTemplate;
            if (template is DataTemplateSelector selector)
                template = selector.SelectTemplate(item, null);

            if (template.CreateContent() is CellBase cell)
            {
                cell.BindingContext = item;
                _generatedCells.Add(cell);
            }
        }

        RaiseSectionChanged();
    }

    #endregion

    #region BindingContext Propagation

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        foreach (var cell in _cells)
            SetInheritedBindingContext(cell, BindingContext);
        foreach (var cell in _generatedCells)
            SetInheritedBindingContext(cell, BindingContext);
    }

    #endregion

    #region Events

    public event EventHandler? SectionChanged;

    internal void RaiseSectionChanged()
    {
        SectionChanged?.Invoke(this, EventArgs.Empty);
    }

    #endregion
}
