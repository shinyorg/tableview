using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sample.App.Pages;

public partial class PickerDemoPage : ContentPage
{
    public PickerDemoPage()
    {
        InitializeComponent();
        BindingContext = new PickerDemoViewModel();
    }
}

public class PickerDemoViewModel : INotifyPropertyChanged
{
    private int _selectedColorIndex;
    private object? _selectedCountry;
    private IList? _selectedHobbies;
    private DateTime _startDate = DateTime.Today;
    private TimeSpan _reminderTime = new(9, 0, 0);
    private int _repeatCount = 1;

    public IList<string> Colors { get; } = new List<string>
    {
        "Red", "Green", "Blue", "Yellow", "Purple", "Orange"
    };

    public IList<string> Countries { get; } = new List<string>
    {
        "United States", "Canada", "United Kingdom", "Germany", "France",
        "Japan", "Australia", "Brazil", "India", "South Korea"
    };

    public IList<string> Hobbies { get; } = new List<string>
    {
        "Reading", "Gaming", "Cooking", "Hiking", "Photography",
        "Music", "Painting", "Cycling", "Swimming", "Gardening"
    };

    public int SelectedColorIndex { get => _selectedColorIndex; set => SetProperty(ref _selectedColorIndex, value); }
    public object? SelectedCountry { get => _selectedCountry; set => SetProperty(ref _selectedCountry, value); }
    public IList? SelectedHobbies { get => _selectedHobbies; set => SetProperty(ref _selectedHobbies, value); }
    public DateTime StartDate { get => _startDate; set => SetProperty(ref _startDate, value); }
    public TimeSpan ReminderTime { get => _reminderTime; set => SetProperty(ref _reminderTime, value); }
    public int RepeatCount { get => _repeatCount; set => SetProperty(ref _repeatCount, value); }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
