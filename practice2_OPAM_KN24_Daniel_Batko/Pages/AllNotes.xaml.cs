using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using System.Collections.ObjectModel;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class AllNotes : Page
{
    private ObservableCollection<Notes> Notes { get; } = new ObservableCollection<Notes>();
    public AllNotes()
    {
        this.InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (e.Parameter is Notes newNotes)
        {
            Notes.Add(newNotes);
        }
    }
}
