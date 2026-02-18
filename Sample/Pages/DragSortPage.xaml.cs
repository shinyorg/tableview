using System.Windows.Input;
using Shiny.Maui.TableView.Controls;

namespace Sample.App.Pages;

public partial class DragSortPage : ContentPage
{
    public DragSortPage()
    {
        InitializeComponent();
        BindingContext = new DragSortViewModel();
    }
}

public class DragSortViewModel
{
    public ICommand ItemDroppedCommand { get; }

    public DragSortViewModel()
    {
        ItemDroppedCommand = new Command<ItemDroppedEventArgs>(args =>
        {
            System.Diagnostics.Debug.WriteLine($"Moved item from index {args.FromIndex} to {args.ToIndex}");
        });
    }
}
