using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Shiny.Maui.TableView.Cells;
using Shiny.Maui.TableView.Infrastructure;
using TvTableSection = Shiny.Maui.TableView.Sections.TableSection;
using TvTableRoot = Shiny.Maui.TableView.Sections.TableRoot;

namespace Shiny.Maui.TableView.Controls;

[ContentProperty(nameof(Root))]
public partial class TableView : ContentView
{
    private ScrollView _scrollView = default!;
    private VerticalStackLayout _rootLayout = default!;
    private TvTableRoot _root = default!;
    private bool _isRendering;
    private INotifyCollectionChanged? _viewItemsSourceNotifier;
    private readonly List<TvTableSection> _generatedSections = new();
    internal bool SuppressRender { get; set; }

    public TableView()
    {
        _root = new TvTableRoot();
        _root.SetParentTableView(this);
        _root.RootChanged += OnRootChanged;

        _scrollView = new ScrollView();
        _rootLayout = new VerticalStackLayout();
        _scrollView.Content = _rootLayout;
        Content = _scrollView;
    }

    #region Root

    public TvTableRoot Root
    {
        get => _root;
        set
        {
            if (_root != null)
            {
                _root.RootChanged -= OnRootChanged;
                _root.SetParentTableView(null);
            }

            _root = value ?? new TvTableRoot();
            _root.SetParentTableView(this);
            _root.RootChanged += OnRootChanged;
            RenderSections();
        }
    }

    #endregion

    #region Bindable Properties

    public static readonly BindableProperty ShowSectionSeparatorProperty = BindableProperty.Create(
        nameof(ShowSectionSeparator), typeof(bool), typeof(TableView), true,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty SectionSeparatorHeightProperty = BindableProperty.Create(
        nameof(SectionSeparatorHeight), typeof(double), typeof(TableView), 8d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty SectionSeparatorColorProperty = BindableProperty.Create(
        nameof(SectionSeparatorColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty SeparatorColorProperty = BindableProperty.Create(
        nameof(SeparatorColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty SeparatorHeightProperty = BindableProperty.Create(
        nameof(SeparatorHeight), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty SeparatorPaddingProperty = BindableProperty.Create(
        nameof(SeparatorPadding), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty ItemDroppedCommandProperty = BindableProperty.Create(
        nameof(ItemDroppedCommand), typeof(ICommand), typeof(TableView), null);

    public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
        nameof(ItemsSource), typeof(IEnumerable), typeof(TableView), null,
        propertyChanged: OnViewItemsSourceChanged);

    public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
        nameof(ItemTemplate), typeof(DataTemplate), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RegenerateTemplatedSections());

    public static readonly BindableProperty TemplateStartIndexProperty = BindableProperty.Create(
        nameof(TemplateStartIndex), typeof(int), typeof(TableView), 0,
        propertyChanged: (b, o, n) => ((TableView)b).RegenerateTemplatedSections());

    public static readonly BindableProperty ScrollToTopProperty = BindableProperty.Create(
        nameof(ScrollToTop), typeof(bool), typeof(TableView), false,
        propertyChanged: async (b, o, n) =>
        {
            if ((bool)n)
            {
                var tv = (TableView)b;
                await tv.ScrollToTopAsync();
                tv.ScrollToTop = false;
            }
        });

    public static readonly BindableProperty ScrollToBottomProperty = BindableProperty.Create(
        nameof(ScrollToBottom), typeof(bool), typeof(TableView), false,
        propertyChanged: async (b, o, n) =>
        {
            if ((bool)n)
            {
                var tv = (TableView)b;
                await tv.ScrollToBottomAsync();
                tv.ScrollToBottom = false;
            }
        });

    #endregion

    #region Properties

    public bool ShowSectionSeparator
    {
        get => (bool)GetValue(ShowSectionSeparatorProperty);
        set => SetValue(ShowSectionSeparatorProperty, value);
    }

    public double SectionSeparatorHeight
    {
        get => (double)GetValue(SectionSeparatorHeightProperty);
        set => SetValue(SectionSeparatorHeightProperty, value);
    }

    public Color? SectionSeparatorColor
    {
        get => (Color?)GetValue(SectionSeparatorColorProperty);
        set => SetValue(SectionSeparatorColorProperty, value);
    }

    public Color? SeparatorColor
    {
        get => (Color?)GetValue(SeparatorColorProperty);
        set => SetValue(SeparatorColorProperty, value);
    }

    public double SeparatorHeight
    {
        get => (double)GetValue(SeparatorHeightProperty);
        set => SetValue(SeparatorHeightProperty, value);
    }

    public double SeparatorPadding
    {
        get => (double)GetValue(SeparatorPaddingProperty);
        set => SetValue(SeparatorPaddingProperty, value);
    }

    public ICommand? ItemDroppedCommand
    {
        get => (ICommand?)GetValue(ItemDroppedCommandProperty);
        set => SetValue(ItemDroppedCommandProperty, value);
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

    public bool ScrollToTop
    {
        get => (bool)GetValue(ScrollToTopProperty);
        set => SetValue(ScrollToTopProperty, value);
    }

    public bool ScrollToBottom
    {
        get => (bool)GetValue(ScrollToBottomProperty);
        set => SetValue(ScrollToBottomProperty, value);
    }

    #endregion

    #region Events

    public event EventHandler<ItemDroppedEventArgs>? ItemDropped;
    public event EventHandler? ModelChanged;
    public event EventHandler<CellPropertyChangedEventArgs>? CellPropertyChanged;

    #endregion

    #region BindingContext Propagation

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (_root != null)
            SetInheritedBindingContext(_root, BindingContext);
    }

    #endregion

    #region Rendering

    private void OnRootChanged(object? sender, EventArgs e)
    {
        RenderSections();
        ModelChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void RenderSections()
    {
        if (_isRendering || SuppressRender)
            return;

        _isRendering = true;

        try
        {
            _rootLayout.Children.Clear();

            var allSections = GetAllSections();

            for (int i = 0; i < allSections.Count; i++)
            {
                var section = allSections[i];
                var sectionView = SectionRenderer.Render(section, this);
                _rootLayout.Children.Add(sectionView);

                // Section separator
                if (ShowSectionSeparator && i < allSections.Count - 1)
                {
                    _rootLayout.Children.Add(new BoxView
                    {
                        HeightRequest = SectionSeparatorHeight,
                        Color = SectionSeparatorColor ?? Colors.Transparent,
                    });
                }
            }
        }
        finally
        {
            _isRendering = false;
        }
    }

    internal IReadOnlyList<TvTableSection> GetAllSections()
    {
        var staticSections = _root.Sections;

        if (_generatedSections.Count == 0)
            return staticSections;

        var result = new List<TvTableSection>();
        var insertIndex = Math.Min(TemplateStartIndex, staticSections.Count);

        for (int i = 0; i < insertIndex; i++)
            result.Add(staticSections[i]);

        result.AddRange(_generatedSections);

        for (int i = insertIndex; i < staticSections.Count; i++)
            result.Add(staticSections[i]);

        return result;
    }

    #endregion

    #region View-Level ItemsSource

    private static void OnViewItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var tv = (TableView)bindable;

        if (tv._viewItemsSourceNotifier != null)
        {
            tv._viewItemsSourceNotifier.CollectionChanged -= tv.OnViewItemsSourceCollectionChanged;
            tv._viewItemsSourceNotifier = null;
        }

        if (newValue is INotifyCollectionChanged notifier)
        {
            tv._viewItemsSourceNotifier = notifier;
            notifier.CollectionChanged += tv.OnViewItemsSourceCollectionChanged;
        }

        tv.RegenerateTemplatedSections();
    }

    private void OnViewItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RegenerateTemplatedSections();
    }

    private void RegenerateTemplatedSections()
    {
        foreach (var section in _generatedSections)
        {
            section.SectionChanged -= OnGeneratedSectionChanged;
            section.ParentTableView = null;
        }

        _generatedSections.Clear();

        if (ItemsSource == null || ItemTemplate == null)
        {
            RenderSections();
            return;
        }

        foreach (var item in ItemsSource)
        {
            var template = ItemTemplate;
            if (template is DataTemplateSelector selector)
                template = selector.SelectTemplate(item, null);

            if (template.CreateContent() is TvTableSection section)
            {
                section.BindingContext = item;
                section.ParentTableView = this;
                section.SectionChanged += OnGeneratedSectionChanged;
                _generatedSections.Add(section);
            }
        }

        RenderSections();
    }

    private void OnGeneratedSectionChanged(object? sender, EventArgs e)
    {
        RenderSections();
    }

    #endregion

    #region Drag & Drop

    internal void RaiseItemDropped(TvTableSection section, CellBase cell, int fromIndex, int toIndex)
    {
        var args = new ItemDroppedEventArgs(section, cell, fromIndex, toIndex);
        ItemDropped?.Invoke(this, args);
        if (ItemDroppedCommand?.CanExecute(args) == true)
            ItemDroppedCommand.Execute(args);
    }

    #endregion

    #region Cell Property Changed

    internal void RaiseCellPropertyChanged(TvTableSection section, CellBase cell, string propertyName)
    {
        CellPropertyChanged?.Invoke(this, new CellPropertyChangedEventArgs(section, cell, propertyName));
    }

    #endregion

    #region Scroll Helpers

    public Task ScrollToTopAsync(bool animated = true)
        => _scrollView.ScrollToAsync(0, 0, animated);

    public Task ScrollToBottomAsync(bool animated = true)
        => _scrollView.ScrollToAsync(0, _rootLayout.Height, animated);

    public double VisibleContentHeight => _scrollView.ContentSize.Height;

    #endregion
}

public class ItemDroppedEventArgs : EventArgs
{
    public TvTableSection Section { get; }
    public CellBase Cell { get; }
    public int FromIndex { get; }
    public int ToIndex { get; }

    public ItemDroppedEventArgs(TvTableSection section, CellBase cell, int fromIndex, int toIndex)
    {
        Section = section;
        Cell = cell;
        FromIndex = fromIndex;
        ToIndex = toIndex;
    }
}

public class CellPropertyChangedEventArgs : EventArgs
{
    public TvTableSection Section { get; }
    public CellBase Cell { get; }
    public string PropertyName { get; }

    public CellPropertyChangedEventArgs(TvTableSection section, CellBase cell, string propertyName)
    {
        Section = section;
        Cell = cell;
        PropertyName = propertyName;
    }
}
