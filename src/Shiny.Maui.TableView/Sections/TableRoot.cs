using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using TvTableView = Shiny.Maui.TableView.Controls.TableView;

namespace Shiny.Maui.TableView.Sections;

[ContentProperty(nameof(Sections))]
public class TableRoot : BindableObject
{
    private readonly ObservableCollection<TableSection> _sections = new();

    public TableRoot()
    {
        _sections.CollectionChanged += OnSectionsCollectionChanged;
    }

    public ObservableCollection<TableSection> Sections => _sections;

    public event EventHandler? RootChanged;

    private void OnSectionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (TableSection section in e.OldItems)
            {
                section.SectionChanged -= OnSectionChanged;
                section.ParentTableView = null;
            }
        }

        if (e.NewItems != null)
        {
            foreach (TableSection section in e.NewItems)
            {
                section.SectionChanged += OnSectionChanged;
                section.ParentTableView = ParentTableView;
            }
        }

        RaiseRootChanged();
    }

    private void OnSectionChanged(object? sender, EventArgs e)
    {
        RaiseRootChanged();
    }

    private void RaiseRootChanged()
    {
        RootChanged?.Invoke(this, EventArgs.Empty);
    }

    internal TvTableView? ParentTableView { get; set; }

    internal void SetParentTableView(TvTableView? tableView)
    {
        ParentTableView = tableView;
        foreach (var section in _sections)
            section.ParentTableView = tableView;
    }
}
