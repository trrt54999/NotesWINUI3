using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class AllNotes : Page, INotifyPropertyChanged
{
    private ObservableCollection<Notes> Notes { get; } = new ObservableCollection<Notes>();

    private string _searchText = string.Empty;

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public AllNotes()
    {
        this.InitializeComponent();
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        const string sharedImagePath = "ms-appx:///Assets/NotesDefaultLogo.png";

        var NotesToAdd = new[]
        {
            new Notes(
                title: "Hi",
                category: "Work",
                content: "Real for sure",
                imagePath: sharedImagePath),

            new Notes(
                title: "IIdk",
                category: "Work",
                content: "Ahahaha",
                imagePath: sharedImagePath),

             new Notes(
                title: "DO ITdsadasasdasdasd",
                category: "Tasks",
                content: "JUST dasasdasdDO IT BROdsaasdasdasdasdO",
                imagePath: sharedImagePath) // TODO: ПОЧИНИТИ СПИСОК, РОЗІБРАТИСЯ З УСІМА НОТАТКАМИ
        };

        foreach (var notes in NotesToAdd)
        {
            Notes.Add(notes);
        }

        if (e.Parameter is Notes newNotes)
        {
            Notes.Add(newNotes);
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        var _notes = new List<Notes>(Notes);
        Notes.Clear();
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            foreach (var note in _notes)
            {
                Notes.Add(note);
            }
            return;
        }

        foreach (var notes in _notes)
        {
            if (notes.NotesTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                Notes.Add(notes);
            }
            else
            {
                Notes.Remove(notes);
            }
        }
    }

}
