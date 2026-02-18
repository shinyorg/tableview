using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Sample.App.Pages;

public partial class BasicSettingsPage : ContentPage
{
    public BasicSettingsPage()
    {
        InitializeComponent();
        BindingContext = new BasicSettingsViewModel();
    }
}

public class BasicSettingsViewModel : INotifyPropertyChanged
{
    private bool _wifiEnabled = true;
    private bool _bluetoothEnabled;
    private bool _termsAccepted;
    private bool _darkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _email = string.Empty;
    private string _selectedTheme = "System";
    private int? _fontSizeValue = 14;
    private TimeSpan _alarmTime = new(7, 0, 0);
    private DateTime? _birthDate = new DateTime(1990, 1, 1);

    public bool WifiEnabled { get => _wifiEnabled; set => SetProperty(ref _wifiEnabled, value); }
    public bool BluetoothEnabled { get => _bluetoothEnabled; set => SetProperty(ref _bluetoothEnabled, value); }
    public bool TermsAccepted { get => _termsAccepted; set => SetProperty(ref _termsAccepted, value); }
    public bool DarkMode
    {
        get => _darkMode;
        set
        {
            if (SetProperty(ref _darkMode, value))
            {
                if (Application.Current != null)
                    Application.Current.UserAppTheme = value ? AppTheme.Dark : AppTheme.Light;
            }
        }
    }
    public string Username { get => _username; set => SetProperty(ref _username, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string Email { get => _email; set => SetProperty(ref _email, value); }
    public string SelectedTheme { get => _selectedTheme; set => SetProperty(ref _selectedTheme, value); }
    public int? FontSizeValue { get => _fontSizeValue; set => SetProperty(ref _fontSizeValue, value); }
    public TimeSpan AlarmTime { get => _alarmTime; set => SetProperty(ref _alarmTime, value); }
    public DateTime? BirthDate { get => _birthDate; set => SetProperty(ref _birthDate, value); }

    public ICommand AboutCommand { get; }
    public ICommand PrivacyCommand { get; }
    public ICommand ResetCommand { get; }

    public BasicSettingsViewModel()
    {
        AboutCommand = new Command(async () =>
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("About", "Shiny.Maui.TableView Sample v1.0", "OK"));
        PrivacyCommand = new Command(async () =>
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Privacy", "Your data is safe.", "OK"));
        ResetCommand = new Command(() =>
        {
            WifiEnabled = true;
            BluetoothEnabled = false;
            TermsAccepted = false;
            DarkMode = false;
            Username = string.Empty;
            Password = string.Empty;
            Email = string.Empty;
            SelectedTheme = "System";
            FontSizeValue = (int?)14;
            AlarmTime = new TimeSpan(7, 0, 0);
            BirthDate = (DateTime?)new DateTime(1990, 1, 1);
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
