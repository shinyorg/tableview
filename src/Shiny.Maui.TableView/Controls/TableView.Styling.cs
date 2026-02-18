using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using TvTableSection = Shiny.Maui.TableView.Sections.TableSection;

namespace Shiny.Maui.TableView.Controls;

public partial class TableView
{
    private static void OnGlobalStyleChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((TableView)bindable).ReapplyCascadedStyles();
    }

    private void ReapplyCascadedStyles()
    {
        foreach (var section in GetAllSections())
        {
            foreach (var cell in section.GetVisibleCells())
            {
                cell.ParentTableView = this;
                cell.ApplyCascadedStyles();
            }
        }
    }

    #region Header Styling

    public static readonly BindableProperty HeaderBackgroundColorProperty = BindableProperty.Create(
        nameof(HeaderBackgroundColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderTextColorProperty = BindableProperty.Create(
        nameof(HeaderTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderFontSizeProperty = BindableProperty.Create(
        nameof(HeaderFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty HeaderPaddingProperty = BindableProperty.Create(
        nameof(HeaderPadding), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

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

    public double HeaderPadding
    {
        get => (double)GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }

    #endregion

    #region Footer Styling

    public static readonly BindableProperty FooterTextColorProperty = BindableProperty.Create(
        nameof(FooterTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty FooterFontSizeProperty = BindableProperty.Create(
        nameof(FooterFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

    public static readonly BindableProperty FooterPaddingProperty = BindableProperty.Create(
        nameof(FooterPadding), typeof(double), typeof(TableView), -1d,
        propertyChanged: (b, o, n) => ((TableView)b).RenderSections());

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

    public double FooterPadding
    {
        get => (double)GetValue(FooterPaddingProperty);
        set => SetValue(FooterPaddingProperty, value);
    }

    #endregion

    #region Cell Title Styling

    public static readonly BindableProperty CellTitleColorProperty = BindableProperty.Create(
        nameof(CellTitleColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellTitleFontSizeProperty = BindableProperty.Create(
        nameof(CellTitleFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellTitleFontAttributesProperty = BindableProperty.Create(
        nameof(CellTitleFontAttributes), typeof(FontAttributes?), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellTitleColor
    {
        get => (Color?)GetValue(CellTitleColorProperty);
        set => SetValue(CellTitleColorProperty, value);
    }

    public double CellTitleFontSize
    {
        get => (double)GetValue(CellTitleFontSizeProperty);
        set => SetValue(CellTitleFontSizeProperty, value);
    }

    public FontAttributes? CellTitleFontAttributes
    {
        get => (FontAttributes?)GetValue(CellTitleFontAttributesProperty);
        set => SetValue(CellTitleFontAttributesProperty, value);
    }

    #endregion

    #region Cell Description Styling

    public static readonly BindableProperty CellDescriptionColorProperty = BindableProperty.Create(
        nameof(CellDescriptionColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellDescriptionFontSizeProperty = BindableProperty.Create(
        nameof(CellDescriptionFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellDescriptionColor
    {
        get => (Color?)GetValue(CellDescriptionColorProperty);
        set => SetValue(CellDescriptionColorProperty, value);
    }

    public double CellDescriptionFontSize
    {
        get => (double)GetValue(CellDescriptionFontSizeProperty);
        set => SetValue(CellDescriptionFontSizeProperty, value);
    }

    #endregion

    #region Cell Hint Styling

    public static readonly BindableProperty CellHintTextColorProperty = BindableProperty.Create(
        nameof(CellHintTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellHintTextFontSizeProperty = BindableProperty.Create(
        nameof(CellHintTextFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellHintTextColor
    {
        get => (Color?)GetValue(CellHintTextColorProperty);
        set => SetValue(CellHintTextColorProperty, value);
    }

    public double CellHintTextFontSize
    {
        get => (double)GetValue(CellHintTextFontSizeProperty);
        set => SetValue(CellHintTextFontSizeProperty, value);
    }

    #endregion

    #region Cell Icon Styling

    public static readonly BindableProperty CellIconSizeProperty = BindableProperty.Create(
        nameof(CellIconSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellIconRadiusProperty = BindableProperty.Create(
        nameof(CellIconRadius), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public double CellIconSize
    {
        get => (double)GetValue(CellIconSizeProperty);
        set => SetValue(CellIconSizeProperty, value);
    }

    public double CellIconRadius
    {
        get => (double)GetValue(CellIconRadiusProperty);
        set => SetValue(CellIconRadiusProperty, value);
    }

    #endregion

    #region Cell Background / Selection Styling

    public static readonly BindableProperty CellBackgroundColorProperty = BindableProperty.Create(
        nameof(CellBackgroundColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellSelectedColorProperty = BindableProperty.Create(
        nameof(CellSelectedColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellBackgroundColor
    {
        get => (Color?)GetValue(CellBackgroundColorProperty);
        set => SetValue(CellBackgroundColorProperty, value);
    }

    public Color? CellSelectedColor
    {
        get => (Color?)GetValue(CellSelectedColorProperty);
        set => SetValue(CellSelectedColorProperty, value);
    }

    #endregion

    #region Cell Value Styling

    public static readonly BindableProperty CellValueTextColorProperty = BindableProperty.Create(
        nameof(CellValueTextColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public static readonly BindableProperty CellValueTextFontSizeProperty = BindableProperty.Create(
        nameof(CellValueTextFontSize), typeof(double), typeof(TableView), -1d,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellValueTextColor
    {
        get => (Color?)GetValue(CellValueTextColorProperty);
        set => SetValue(CellValueTextColorProperty, value);
    }

    public double CellValueTextFontSize
    {
        get => (double)GetValue(CellValueTextFontSizeProperty);
        set => SetValue(CellValueTextFontSizeProperty, value);
    }

    #endregion

    #region Cell Accessory Styling

    public static readonly BindableProperty CellAccentColorProperty = BindableProperty.Create(
        nameof(CellAccentColor), typeof(Color), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Color? CellAccentColor
    {
        get => (Color?)GetValue(CellAccentColorProperty);
        set => SetValue(CellAccentColorProperty, value);
    }

    #endregion

    #region Cell Padding

    public static readonly BindableProperty CellPaddingProperty = BindableProperty.Create(
        nameof(CellPadding), typeof(Thickness?), typeof(TableView), null,
        propertyChanged: OnGlobalStyleChanged);

    public Thickness? CellPadding
    {
        get => (Thickness?)GetValue(CellPaddingProperty);
        set => SetValue(CellPaddingProperty, value);
    }

    #endregion
}
