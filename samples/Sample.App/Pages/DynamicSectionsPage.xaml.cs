using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Sample.App.Pages;

public partial class DynamicSectionsPage : ContentPage
{
    public DynamicSectionsPage()
    {
        InitializeComponent();
        BindingContext = new DynamicSectionsViewModel();
    }
}

public class DynamicSectionsViewModel : INotifyPropertyChanged
{
    private int _counter = 1;

    public ObservableCollection<DynamicItem> Items { get; } = new()
    {
        new DynamicItem { Name = "Item 1", Value = "Value 1" },
        new DynamicItem { Name = "Item 2", Value = "Value 2" },
        new DynamicItem { Name = "Item 3", Value = "Value 3" }
    };

    public ICommand AddCommand { get; }
    public ICommand RemoveCommand { get; }

    public DynamicSectionsViewModel()
    {
        AddCommand = new Command(() =>
        {
            _counter++;
            Items.Add(new DynamicItem { Name = $"Item {Items.Count + 1}", Value = $"Value {_counter}" });
        });

        RemoveCommand = new Command(() =>
        {
            if (Items.Count > 0)
                Items.RemoveAt(Items.Count - 1);
        });
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}

public class DynamicItem : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _value = string.Empty;

    public string Name
    {
        get => _name;
        set { _name = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name))); }
    }

    public string Value
    {
        get => _value;
        set { _value = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value))); }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
