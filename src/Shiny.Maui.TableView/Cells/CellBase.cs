using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;
using TvTableSection = Shiny.Maui.TableView.Sections.TableSection;

namespace Shiny.Maui.TableView.Cells;

public abstract class CellBase : ContentView
{
    private Grid _rootGrid = default!;
    private Image _iconImage = default!;
    private Label _titleLabel = default!;
    private Label _descriptionLabel = default!;
    private Label _hintLabel = default!;
    private View? _accessoryView;

    public CellBase()
    {
        BuildLayout();
        var tapGesture = new TapGestureRecognizer();
        tapGesture.Tapped += OnCellTapped;
        GestureRecognizers.Add(tapGesture);
    }

    #region Bindable Properties

    public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(
        nameof(IconSource), typeof(ImageSource), typeof(CellBase), null,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateIconVisibility());

    public static readonly BindableProperty IconSizeProperty = BindableProperty.Create(
        nameof(IconSize), typeof(double), typeof(CellBase), -1d,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateIconSize());

    public static readonly BindableProperty IconRadiusProperty = BindableProperty.Create(
        nameof(IconRadius), typeof(double), typeof(CellBase), -1d);

    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(CellBase), string.Empty,
        propertyChanged: (b, o, n) => ((CellBase)b)._titleLabel.Text = (string)n);

    public static readonly BindableProperty TitleColorProperty = BindableProperty.Create(
        nameof(TitleColor), typeof(Color), typeof(CellBase), null,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateTitleColor());

    public static readonly BindableProperty TitleFontSizeProperty = BindableProperty.Create(
        nameof(TitleFontSize), typeof(double), typeof(CellBase), -1d,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateTitleFontSize());

    public static readonly BindableProperty TitleFontAttributesProperty = BindableProperty.Create(
        nameof(TitleFontAttributes), typeof(FontAttributes?), typeof(CellBase), null,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateTitleFontAttributes());

    public static readonly BindableProperty DescriptionProperty = BindableProperty.Create(
        nameof(Description), typeof(string), typeof(CellBase), string.Empty,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateDescription());

    public static readonly BindableProperty DescriptionColorProperty = BindableProperty.Create(
        nameof(DescriptionColor), typeof(Color), typeof(CellBase), null,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateDescriptionColor());

    public static readonly BindableProperty DescriptionFontSizeProperty = BindableProperty.Create(
        nameof(DescriptionFontSize), typeof(double), typeof(CellBase), -1d,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateDescriptionFontSize());

    public static readonly BindableProperty HintTextProperty = BindableProperty.Create(
        nameof(HintText), typeof(string), typeof(CellBase), string.Empty,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateHintText());

    public static readonly BindableProperty HintTextColorProperty = BindableProperty.Create(
        nameof(HintTextColor), typeof(Color), typeof(CellBase), null,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateHintTextColor());

    public static readonly BindableProperty HintTextFontSizeProperty = BindableProperty.Create(
        nameof(HintTextFontSize), typeof(double), typeof(CellBase), -1d,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateHintTextFontSize());

    public static readonly BindableProperty CellBackgroundColorProperty = BindableProperty.Create(
        nameof(CellBackgroundColor), typeof(Color), typeof(CellBase), null,
        propertyChanged: (b, o, n) => ((CellBase)b).UpdateCellBackground());

    public static readonly BindableProperty SelectedColorProperty = BindableProperty.Create(
        nameof(SelectedColor), typeof(Color), typeof(CellBase), null);

    public static readonly BindableProperty IsSelectableProperty = BindableProperty.Create(
        nameof(IsSelectable), typeof(bool), typeof(CellBase), true);

    #endregion

    #region Properties

    public ImageSource? IconSource
    {
        get => (ImageSource?)GetValue(IconSourceProperty);
        set => SetValue(IconSourceProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public double IconRadius
    {
        get => (double)GetValue(IconRadiusProperty);
        set => SetValue(IconRadiusProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Color? TitleColor
    {
        get => (Color?)GetValue(TitleColorProperty);
        set => SetValue(TitleColorProperty, value);
    }

    public double TitleFontSize
    {
        get => (double)GetValue(TitleFontSizeProperty);
        set => SetValue(TitleFontSizeProperty, value);
    }

    public FontAttributes? TitleFontAttributes
    {
        get => (FontAttributes?)GetValue(TitleFontAttributesProperty);
        set => SetValue(TitleFontAttributesProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public Color? DescriptionColor
    {
        get => (Color?)GetValue(DescriptionColorProperty);
        set => SetValue(DescriptionColorProperty, value);
    }

    public double DescriptionFontSize
    {
        get => (double)GetValue(DescriptionFontSizeProperty);
        set => SetValue(DescriptionFontSizeProperty, value);
    }

    public string HintText
    {
        get => (string)GetValue(HintTextProperty);
        set => SetValue(HintTextProperty, value);
    }

    public Color? HintTextColor
    {
        get => (Color?)GetValue(HintTextColorProperty);
        set => SetValue(HintTextColorProperty, value);
    }

    public double HintTextFontSize
    {
        get => (double)GetValue(HintTextFontSizeProperty);
        set => SetValue(HintTextFontSizeProperty, value);
    }

    public Color? CellBackgroundColor
    {
        get => (Color?)GetValue(CellBackgroundColorProperty);
        set => SetValue(CellBackgroundColorProperty, value);
    }

    public Color? SelectedColor
    {
        get => (Color?)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public bool IsSelectable
    {
        get => (bool)GetValue(IsSelectableProperty);
        set => SetValue(IsSelectableProperty, value);
    }

    #endregion

    #region Parent References

    internal TvTableView? ParentTableView { get; set; }
    internal TvTableSection? ParentSection { get; set; }

    #endregion

    #region Layout Building

    private void BuildLayout()
    {
        _rootGrid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),   // Icon
                new ColumnDefinition(GridLength.Star),    // Title/Description
                new ColumnDefinition(GridLength.Auto)     // Accessory
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto)
            },
            Padding = new Thickness(16, 12),
            ColumnSpacing = 12,
            RowSpacing = 2
        };

        // Icon
        _iconImage = new Image
        {
            WidthRequest = 24,
            HeightRequest = 24,
            VerticalOptions = LayoutOptions.Center,
            IsVisible = false
        };
        Grid.SetColumn(_iconImage, 0);
        Grid.SetRowSpan(_iconImage, 2);
        _rootGrid.Children.Add(_iconImage);

        // Title
        _titleLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            LineBreakMode = LineBreakMode.TailTruncation
        };
        Grid.SetColumn(_titleLabel, 1);
        Grid.SetRow(_titleLabel, 0);
        _rootGrid.Children.Add(_titleLabel);

        // Hint (top-right of title area)
        _hintLabel = new Label
        {
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.End,
            FontSize = 12,
            IsVisible = false
        };
        Grid.SetColumn(_hintLabel, 1);
        Grid.SetRow(_hintLabel, 0);
        _rootGrid.Children.Add(_hintLabel);

        // Description
        _descriptionLabel = new Label
        {
            VerticalOptions = LayoutOptions.Start,
            LineBreakMode = LineBreakMode.WordWrap,
            FontSize = 12,
            IsVisible = false
        };
        Grid.SetColumn(_descriptionLabel, 1);
        Grid.SetRow(_descriptionLabel, 1);
        _rootGrid.Children.Add(_descriptionLabel);

        // Accessory placeholder - subclasses provide their own
        _accessoryView = CreateAccessoryView();
        if (_accessoryView != null)
        {
            _accessoryView.VerticalOptions = LayoutOptions.Center;
            Grid.SetColumn(_accessoryView, 2);
            Grid.SetRowSpan(_accessoryView, 2);
            _rootGrid.Children.Add(_accessoryView);
        }

        Content = _rootGrid;
    }

    protected virtual View? CreateAccessoryView() => null;

    protected View? AccessoryView => _accessoryView;
    protected Grid RootGrid => _rootGrid;
    protected Label TitleLabel => _titleLabel;
    protected Label DescriptionLabel => _descriptionLabel;
    protected Label HintLabel => _hintLabel;

    #endregion

    #region Property Resolution (Cascading)

    internal void ApplyCascadedStyles()
    {
        UpdateTitleColor();
        UpdateTitleFontSize();
        UpdateTitleFontAttributes();
        UpdateDescriptionColor();
        UpdateDescriptionFontSize();
        UpdateHintTextColor();
        UpdateHintTextFontSize();
        UpdateIconSize();
        UpdateCellBackground();
    }

    protected Color ResolveColor(Color? cellValue, Color? globalValue, Color fallback)
        => cellValue ?? globalValue ?? fallback;

    protected double ResolveDouble(double cellValue, double globalValue, double fallback)
    {
        if (cellValue >= 0) return cellValue;
        if (globalValue >= 0) return globalValue;
        return fallback;
    }

    protected FontAttributes ResolveFontAttributes(FontAttributes? cellValue, FontAttributes? globalValue)
        => cellValue ?? globalValue ?? FontAttributes.None;

    private void UpdateTitleColor()
    {
        _titleLabel.TextColor = ResolveColor(TitleColor, ParentTableView?.CellTitleColor, Colors.Black);
    }

    private void UpdateTitleFontSize()
    {
        _titleLabel.FontSize = ResolveDouble(TitleFontSize, ParentTableView?.CellTitleFontSize ?? -1, 16);
    }

    private void UpdateTitleFontAttributes()
    {
        _titleLabel.FontAttributes = ResolveFontAttributes(TitleFontAttributes, ParentTableView?.CellTitleFontAttributes);
    }

    private void UpdateDescriptionColor()
    {
        _descriptionLabel.TextColor = ResolveColor(DescriptionColor, ParentTableView?.CellDescriptionColor, Colors.Gray);
    }

    private void UpdateDescriptionFontSize()
    {
        _descriptionLabel.FontSize = ResolveDouble(DescriptionFontSize, ParentTableView?.CellDescriptionFontSize ?? -1, 12);
    }

    private void UpdateDescription()
    {
        var text = Description;
        _descriptionLabel.Text = text;
        _descriptionLabel.IsVisible = !string.IsNullOrEmpty(text);
    }

    private void UpdateHintText()
    {
        var text = HintText;
        _hintLabel.Text = text;
        _hintLabel.IsVisible = !string.IsNullOrEmpty(text);
    }

    private void UpdateHintTextColor()
    {
        _hintLabel.TextColor = ResolveColor(HintTextColor, ParentTableView?.CellHintTextColor, Colors.Gray);
    }

    private void UpdateHintTextFontSize()
    {
        _hintLabel.FontSize = ResolveDouble(HintTextFontSize, ParentTableView?.CellHintTextFontSize ?? -1, 12);
    }

    private void UpdateIconVisibility()
    {
        var hasIcon = IconSource != null;
        _iconImage.Source = IconSource;
        _iconImage.IsVisible = hasIcon;
    }

    private void UpdateIconSize()
    {
        var size = ResolveDouble(IconSize, ParentTableView?.CellIconSize ?? -1, 24);
        _iconImage.WidthRequest = size;
        _iconImage.HeightRequest = size;
    }

    private void UpdateCellBackground()
    {
        var color = ResolveColor(CellBackgroundColor, ParentTableView?.CellBackgroundColor, Colors.White);
        BackgroundColor = color;
    }

    #endregion

    #region Tap Handling

    protected virtual void OnCellTapped(object? sender, TappedEventArgs e)
    {
        if (!IsSelectable)
            return;

        ShowTapFeedback();
        OnTapped();
    }

    protected virtual void OnTapped() { }

    private async void ShowTapFeedback()
    {
        var color = SelectedColor ?? ParentTableView?.CellSelectedColor;
        if (color == null)
            return;

        var original = BackgroundColor;
        BackgroundColor = color;
        await Task.Delay(150);
        UpdateCellBackground();
    }

    #endregion
}
