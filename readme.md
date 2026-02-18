# Shiny.Maui.TableView

A settings-style TableView for .NET MAUI, inspired by [AiForms.Maui.SettingsView](https://github.com/muak/AiForms.Maui.SettingsView) by Kaoru Nakamura (muak).

## How It's Different

AiForms.SettingsView uses native platform controls under the hood (`UITableView` on iOS, `RecyclerView` on Android) with custom MAUI handlers. This gives great native fidelity but ties the implementation to platform-specific code.

**Shiny.Maui.TableView takes a pure .NET MAUI approach.** The entire control is built from standard MAUI layouts and views with zero platform-specific code. This means:

- **Single codebase** &mdash; No custom handlers, renderers, or platform projects to maintain
- **Any MAUI target** &mdash; Works on iOS, Android, MacCatalyst, and any future MAUI target without platform-specific work
- **Dark mode support** &mdash; Respects the system theme automatically; no hardcoded colors
- **Fully styleable** &mdash; Cascading style system from TableView &rarr; Section &rarr; Cell with per-cell overrides
- **XAML-first** &mdash; All properties are `BindableProperty` with full MVVM/binding support

The trade-off is that you get MAUI's rendering rather than native list controls, but for settings screens with dozens (not thousands) of items, this is the right trade-off.

## Setup

Install the NuGet package and call `UseShinyTableView()` in your `MauiProgram.cs`:

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseShinyTableView();
```

Add the XML namespace to your XAML pages:

```xml
xmlns:tv="http://shiny.net/maui/tableview"
```

## Quick Start

```xml
<tv:TableView>
    <tv:TableRoot>
        <tv:TableSection Title="Account">
            <tv:SwitchCell Title="Notifications" On="{Binding NotificationsOn, Mode=TwoWay}" />
            <tv:EntryCell Title="Username" ValueText="{Binding Username, Mode=TwoWay}" Placeholder="Enter name" />
            <tv:CommandCell Title="About" Command="{Binding AboutCommand}" ShowArrow="True" />
        </tv:TableSection>
    </tv:TableRoot>
</tv:TableView>
```

## Cell Types

### LabelCell

Displays a title with an optional value text on the right.

```xml
<tv:LabelCell Title="Version" ValueText="1.0.0" Description="Latest stable release" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `ValueText` | `string` | `""` | Text displayed on the right side |
| `ValueTextColor` | `Color?` | `null` | Value text color |
| `ValueTextFontSize` | `double` | `-1` | Value font size (-1 = use global) |
| `ValueTextFontFamily` | `string?` | `null` | Value font family |
| `ValueTextFontAttributes` | `FontAttributes?` | `null` | Bold, Italic, or None |

### SwitchCell

A cell with a toggle switch.

```xml
<tv:SwitchCell Title="Wi-Fi" On="{Binding WifiEnabled, Mode=TwoWay}" OnColor="#34C759" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `On` | `bool` | `false` | Switch state (two-way) |
| `OnColor` | `Color?` | `null` | Switch color when on |

### CheckboxCell

A cell with a native checkbox control.

```xml
<tv:CheckboxCell Title="Accept Terms" Checked="{Binding Accepted, Mode=TwoWay}" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Checked` | `bool` | `false` | Checkbox state (two-way) |
| `AccentColor` | `Color?` | `null` | Checkbox color |

### SimpleCheckCell

A cell that shows/hides a checkmark on tap. Useful for selection lists.

```xml
<tv:SimpleCheckCell Title="Option A" Checked="{Binding OptionA, Mode=TwoWay}" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Checked` | `bool` | `false` | Check state (two-way) |
| `Value` | `object?` | `null` | Associated value |
| `AccentColor` | `Color?` | `null` | Checkmark color |

### RadioCell

Radio button selection within a section. Use the `RadioCell.SelectedValue` attached property on the parent `TableSection` to bind the selected value.

```xml
<tv:TableSection Title="Theme" tv:RadioCell.SelectedValue="{Binding SelectedTheme, Mode=TwoWay}">
    <tv:RadioCell Title="Light" Value="Light" />
    <tv:RadioCell Title="Dark" Value="Dark" />
    <tv:RadioCell Title="System" Value="System" />
</tv:TableSection>
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Value` | `object?` | `null` | The value this radio button represents |
| `AccentColor` | `Color?` | `null` | Radio indicator color |

### CommandCell

A tappable cell with a disclosure arrow that executes a command.

```xml
<tv:CommandCell Title="Privacy Policy"
                 ValueText="View"
                 Command="{Binding PrivacyCommand}"
                 KeepSelectedUntilBack="True" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Command` | `ICommand?` | `null` | Command to execute on tap |
| `CommandParameter` | `object?` | `null` | Parameter passed to command |
| `ShowArrow` | `bool` | `true` | Show disclosure arrow |
| `KeepSelectedUntilBack` | `bool` | `false` | Keep highlight until page reappears |

Inherits all properties from `LabelCell`.

### ButtonCell

A full-width button-style cell.

```xml
<tv:ButtonCell Title="Sign Out" Command="{Binding SignOutCommand}" ButtonTextColor="Red" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Command` | `ICommand?` | `null` | Command to execute |
| `CommandParameter` | `object?` | `null` | Parameter passed to command |
| `ButtonTextColor` | `Color?` | `null` | Button text color |
| `TitleAlignment` | `TextAlignment` | `Center` | Text alignment |

### EntryCell

A cell with an inline text entry field.

```xml
<tv:EntryCell Title="Email"
               ValueText="{Binding Email, Mode=TwoWay}"
               Placeholder="user@example.com"
               Keyboard="Email" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `ValueText` | `string` | `""` | Entry text (two-way) |
| `Placeholder` | `string` | `""` | Placeholder text |
| `PlaceholderColor` | `Color?` | `null` | Placeholder color |
| `Keyboard` | `Keyboard` | `Default` | Keyboard type |
| `IsPassword` | `bool` | `false` | Mask input |
| `MaxLength` | `int` | `-1` | Max characters (-1 = unlimited) |
| `TextAlignment` | `TextAlignment` | `End` | Text alignment |
| `CompletedCommand` | `ICommand?` | `null` | Command on return key |

### DatePickerCell

Tapping the cell opens the native date picker dialog.

```xml
<tv:DatePickerCell Title="Birthday"
                    Date="{Binding BirthDate, Mode=TwoWay}"
                    Format="D"
                    MinimumDate="1900-01-01"
                    MaximumDate="2025-12-31" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Date` | `DateTime?` | `null` | Selected date (two-way) |
| `InitialDate` | `DateTime` | `2000-01-01` | Date shown when no date is set |
| `MinimumDate` | `DateTime` | `1900-01-01` | Earliest selectable date |
| `MaximumDate` | `DateTime` | `2100-12-31` | Latest selectable date |
| `Format` | `string` | `"d"` | Date format string |
| `ValueTextColor` | `Color?` | `null` | Value text color |

### TimePickerCell

Tapping the cell opens the native time picker dialog.

```xml
<tv:TimePickerCell Title="Alarm"
                    Time="{Binding AlarmTime, Mode=TwoWay}"
                    Format="T" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Time` | `TimeSpan` | `00:00:00` | Selected time (two-way) |
| `Format` | `string` | `"t"` | Time format string |
| `ValueTextColor` | `Color?` | `null` | Value text color |

### TextPickerCell

Tapping the cell opens a native dropdown/spinner picker.

```xml
<tv:TextPickerCell Title="Color"
                    ItemsSource="{Binding Colors}"
                    SelectedIndex="{Binding SelectedColorIndex, Mode=TwoWay}"
                    PickerTitle="Choose a color" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `ItemsSource` | `IList?` | `null` | List of items |
| `SelectedIndex` | `int` | `-1` | Selected index (two-way) |
| `SelectedItem` | `object?` | `null` | Selected item (two-way) |
| `DisplayMember` | `string?` | `null` | Property name for display text |
| `PickerTitle` | `string?` | `null` | Title shown on picker |
| `SelectedCommand` | `ICommand?` | `null` | Command on selection |
| `ValueTextColor` | `Color?` | `null` | Value text color |

### NumberPickerCell

Tapping the cell opens a prompt dialog for numeric input.

```xml
<tv:NumberPickerCell Title="Font Size"
                      Number="{Binding FontSize, Mode=TwoWay}"
                      Min="8" Max="72" Unit="pt" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Number` | `int?` | `null` | Selected number (two-way) |
| `Min` | `int` | `0` | Minimum value |
| `Max` | `int` | `9999` | Maximum value |
| `Unit` | `string` | `""` | Unit suffix (e.g., "pt", "px") |
| `PickerTitle` | `string` | `"Enter a number"` | Dialog title |
| `SelectedCommand` | `ICommand?` | `null` | Command on selection |
| `ValueTextColor` | `Color?` | `null` | Value text color |

### PickerCell

A full-page picker for single or multiple selection. Navigates to a selection page on tap.

```xml
<!-- Single selection -->
<tv:PickerCell Title="Country"
                ItemsSource="{Binding Countries}"
                SelectionMode="Single"
                SelectedItem="{Binding SelectedCountry, Mode=TwoWay}"
                PageTitle="Select Country" />

<!-- Multiple selection -->
<tv:PickerCell Title="Hobbies"
                ItemsSource="{Binding Hobbies}"
                SelectionMode="Multiple"
                MaxSelectedNumber="3"
                SelectedItems="{Binding SelectedHobbies, Mode=TwoWay}"
                PageTitle="Select Hobbies" />
```

| Property | Type | Default | Description |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | Items to pick from |
| `SelectedItem` | `object?` | `null` | Selected item (two-way, single mode) |
| `SelectedItems` | `IList?` | `null` | Selected items (two-way, multiple mode) |
| `SelectionMode` | `SelectionMode` | `Single` | Single or Multiple |
| `MaxSelectedNumber` | `int` | `0` | Max selections (0 = unlimited) |
| `UsePickToClose` | `bool` | `false` | Auto-close when max reached |
| `UseAutoValueText` | `bool` | `true` | Auto-generate display text |
| `DisplayMember` | `string?` | `null` | Property for display text |
| `SubDisplayMember` | `string?` | `null` | Property for subtitle |
| `PageTitle` | `string` | `"Select"` | Picker page title |
| `ShowArrow` | `bool` | `true` | Show disclosure arrow |
| `KeepSelectedUntilBack` | `bool` | `false` | Keep highlight until return |
| `SelectedCommand` | `ICommand?` | `null` | Command on selection |
| `AccentColor` | `Color?` | `null` | Checkmark color on picker page |

### CustomCell

A cell that hosts any custom MAUI view.

```xml
<tv:CustomCell Title="Custom Content">
    <tv:CustomCell.CustomContent>
        <ProgressBar Progress="0.75" />
    </tv:CustomCell.CustomContent>
</tv:CustomCell>
```

| Property | Type | Default | Description |
|---|---|---|---|
| `CustomContent` | `View?` | `null` | Custom view |
| `UseFullSize` | `bool` | `false` | Use full cell width for content |
| `Command` | `ICommand?` | `null` | Command on tap |
| `LongCommand` | `ICommand?` | `null` | Command on double-tap |
| `ShowArrow` | `bool` | `false` | Show disclosure arrow |
| `KeepSelectedUntilBack` | `bool` | `false` | Keep highlight until return |

## Common Cell Properties

All cells inherit from `CellBase` and share these properties:

| Property | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Primary text |
| `TitleColor` | `Color?` | `null` | Title color |
| `TitleFontSize` | `double` | `-1` | Title font size |
| `TitleFontFamily` | `string?` | `null` | Title font family |
| `TitleFontAttributes` | `FontAttributes?` | `null` | Title styling |
| `Description` | `string` | `""` | Subtitle text below title |
| `DescriptionColor` | `Color?` | `null` | Description color |
| `DescriptionFontSize` | `double` | `-1` | Description font size |
| `HintText` | `string` | `""` | Hint text on the right of the title |
| `HintTextColor` | `Color?` | `null` | Hint color |
| `IconSource` | `ImageSource?` | `null` | Icon on the left |
| `IconSize` | `double` | `-1` | Icon dimensions |
| `IconRadius` | `double` | `-1` | Icon corner radius |
| `CellBackgroundColor` | `Color?` | `null` | Cell background |
| `SelectedColor` | `Color?` | `null` | Background when tapped |
| `IsSelectable` | `bool` | `true` | Whether the cell responds to taps |
| `IsEnabled` | `bool` | `true` | Disabled cells are dimmed |
| `CellHeight` | `double` | `-1` | Fixed cell height |
| `BorderColor` | `Color?` | `null` | Cell border color |
| `BorderWidth` | `double` | `-1` | Cell border width |
| `BorderRadius` | `double` | `-1` | Cell border corner radius |

## Sections

### TableSection

Groups cells with an optional header and footer.

```xml
<tv:TableSection Title="GENERAL"
                  FooterText="These settings apply globally">
    <tv:LabelCell Title="Item 1" />
    <tv:LabelCell Title="Item 2" />
</tv:TableSection>
```

| Property | Type | Default | Description |
|---|---|---|---|
| `Title` | `string` | `""` | Header text |
| `FooterText` | `string` | `""` | Footer text |
| `HeaderView` | `View?` | `null` | Custom header view (overrides Title) |
| `FooterView` | `View?` | `null` | Custom footer view (overrides FooterText) |
| `IsVisible` | `bool` | `true` | Show/hide entire section |
| `FooterVisible` | `bool` | `true` | Show/hide footer |
| `HeaderBackgroundColor` | `Color?` | `null` | Header background |
| `HeaderTextColor` | `Color?` | `null` | Header text color |
| `HeaderFontSize` | `double` | `-1` | Header font size |
| `HeaderFontFamily` | `string?` | `null` | Header font family |
| `HeaderFontAttributes` | `FontAttributes?` | `null` | Header styling |
| `HeaderHeight` | `double` | `-1` | Fixed header height |
| `FooterTextColor` | `Color?` | `null` | Footer text color |
| `FooterFontSize` | `double` | `-1` | Footer font size |
| `FooterBackgroundColor` | `Color?` | `null` | Footer background |
| `UseDragSort` | `bool` | `false` | Enable reorder controls |

### Dynamic Cells with ItemTemplate

Sections can generate cells from a data source:

```xml
<tv:TableSection Title="Items"
                  ItemsSource="{Binding Items}">
    <tv:TableSection.ItemTemplate>
        <DataTemplate>
            <tv:LabelCell Title="{Binding Name}" ValueText="{Binding Value}" />
        </DataTemplate>
    </tv:TableSection.ItemTemplate>
</tv:TableSection>
```

| Property | Type | Default | Description |
|---|---|---|---|
| `ItemsSource` | `IEnumerable?` | `null` | Data source for generated cells |
| `ItemTemplate` | `DataTemplate?` | `null` | Template for each item |
| `TemplateStartIndex` | `int` | `0` | Insert position among static cells |

## Global Styling

Apply styles to all cells from the `TableView` level. Individual cell properties override global values.

```xml
<tv:TableView CellTitleColor="#333333"
              CellTitleFontSize="17"
              CellDescriptionColor="#888888"
              CellValueTextColor="#007AFF"
              CellBackgroundColor="White"
              CellSelectedColor="#EFEFEF"
              CellAccentColor="#007AFF"
              CellIconSize="28"
              HeaderTextColor="#666666"
              HeaderFontSize="13"
              HeaderBackgroundColor="#F2F2F7"
              FooterTextColor="#8E8E93"
              SeparatorColor="#C6C6C8">
```

### TableView Styling Properties

**Cell styles** (cascade to all cells):

| Property | Type | Description |
|---|---|---|
| `CellTitleColor` | `Color?` | Title text color |
| `CellTitleFontSize` | `double` | Title font size |
| `CellTitleFontFamily` | `string?` | Title font family |
| `CellTitleFontAttributes` | `FontAttributes?` | Title styling |
| `CellDescriptionColor` | `Color?` | Description text color |
| `CellDescriptionFontSize` | `double` | Description font size |
| `CellDescriptionFontFamily` | `string?` | Description font family |
| `CellDescriptionFontAttributes` | `FontAttributes?` | Description styling |
| `CellHintTextColor` | `Color?` | Hint text color |
| `CellHintTextFontSize` | `double` | Hint font size |
| `CellValueTextColor` | `Color?` | Value text color |
| `CellValueTextFontSize` | `double` | Value font size |
| `CellValueTextFontFamily` | `string?` | Value font family |
| `CellValueTextFontAttributes` | `FontAttributes?` | Value styling |
| `CellBackgroundColor` | `Color?` | Cell background |
| `CellSelectedColor` | `Color?` | Background on tap |
| `CellAccentColor` | `Color?` | Accent for switches, checkboxes, radios |
| `CellIconSize` | `double` | Icon dimensions |
| `CellIconRadius` | `double` | Icon corner radius |
| `CellPadding` | `Thickness?` | Cell content padding |
| `CellBorderColor` | `Color?` | Cell border color |
| `CellBorderWidth` | `double` | Cell border width |
| `CellBorderRadius` | `double` | Cell border corner radius |

**Header/Footer styles**:

| Property | Type | Description |
|---|---|---|
| `HeaderBackgroundColor` | `Color?` | Header background |
| `HeaderTextColor` | `Color?` | Header text color |
| `HeaderFontSize` | `double` | Header font size |
| `HeaderFontFamily` | `string?` | Header font family |
| `HeaderFontAttributes` | `FontAttributes` | Header styling (default: Bold) |
| `HeaderPadding` | `Thickness` | Header padding |
| `HeaderHeight` | `double` | Fixed header height |
| `HeaderTextVerticalAlign` | `LayoutAlignment` | Vertical alignment (default: End) |
| `FooterTextColor` | `Color?` | Footer text color |
| `FooterFontSize` | `double` | Footer font size |
| `FooterFontFamily` | `string?` | Footer font family |
| `FooterFontAttributes` | `FontAttributes` | Footer styling |
| `FooterPadding` | `Thickness` | Footer padding |
| `FooterBackgroundColor` | `Color?` | Footer background |

**Separator/Section styles**:

| Property | Type | Default | Description |
|---|---|---|---|
| `SeparatorColor` | `Color?` | `null` | Cell separator color |
| `SeparatorHeight` | `double` | `0.5` | Separator thickness |
| `SeparatorPadding` | `double` | `16` | Separator left inset |
| `ShowSectionSeparator` | `bool` | `true` | Show gap between sections |
| `SectionSeparatorHeight` | `double` | `8` | Gap height between sections |
| `SectionSeparatorColor` | `Color?` | `null` | Gap color |

## Drag & Sort

Enable reorder controls on a section. Each cell gets up/down arrows to move it within the section.

```xml
<tv:TableView ItemDroppedCommand="{Binding ItemDroppedCommand}">
    <tv:TableRoot>
        <tv:TableSection Title="Drag to Reorder" UseDragSort="True">
            <tv:LabelCell Title="First" ValueText="1" />
            <tv:LabelCell Title="Second" ValueText="2" />
            <tv:LabelCell Title="Third" ValueText="3" />
        </tv:TableSection>
    </tv:TableRoot>
</tv:TableView>
```

The `ItemDroppedCommand` receives an `ItemDroppedEventArgs` with `Section`, `Cell`, `FromIndex`, and `ToIndex`.

## Scroll Control

```xml
<!-- Trigger scroll from view model -->
<tv:TableView ScrollToTop="{Binding ShouldScrollTop}" ScrollToBottom="{Binding ShouldScrollBottom}" />
```

```csharp
// Or from code
await tableView.ScrollToTopAsync();
await tableView.ScrollToBottomAsync();
```

## Acknowledgments

This project was inspired by [AiForms.Maui.SettingsView](https://github.com/muak/AiForms.Maui.SettingsView) by **Kaoru Nakamura (muak)**, licensed under MIT. AiForms.SettingsView pioneered the rich settings TableView pattern for Xamarin.Forms and .NET MAUI using native platform renderers (UITableView on iOS, RecyclerView on Android).

Shiny.Maui.TableView reimagines that feature set as a pure .NET MAUI implementation&mdash;no platform-specific code, no custom handlers&mdash;making it simpler to maintain and extend while supporting the same cell types and styling capabilities.

## License

MIT
