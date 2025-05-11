using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class AllNotes : Page, INotifyPropertyChanged
{
    public ObservableCollection<Notes> AllNotesList { get; } = new ObservableCollection<Notes>();
    public ObservableCollection<Notes> Notes { get; } = new ObservableCollection<Notes>();
    private readonly DataService _dataService;

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
        _dataService = new DataService();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        await RefreshAllNotesAsync();

        Frame.Navigated += Frame_Navigated;
    }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        Frame.Navigated -= Frame_Navigated;
        base.OnNavigatedFrom(e);
    }

    private async void Frame_Navigated(object sender, NavigationEventArgs e)
    {
        if (e.SourcePageType == typeof(EditNotePage))
        {
            Debug.WriteLine("Frame navigated from EditNotePage, refreshing notes.");
            await RefreshAllNotesAsync();
        }
    }

    private async Task RefreshAllNotesAsync()
    {
        AllNotesList.Clear();
        var currentUser = App.AuthService?.CurrentUser;
        if (currentUser != null)
        {
            var userNotes = await _dataService.LoadNotesAsync(currentUser.Username);
            foreach (var note in userNotes)
            {
                AllNotesList.Add(note);
            }
            Debug.WriteLine($"Loaded {userNotes.Count} notes for user {currentUser.Username}");
        }
        RefreshVisibleNotes();
    }

    public void RefreshVisibleNotes()
    {
        Notes.Clear();

        List<Notes> filteredNotes = new List<Notes>();

        foreach (var note in AllNotesList)
        {
            if (string.IsNullOrWhiteSpace(SearchText) ||
                note.NotesTitle.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
            {
                filteredNotes.Add(note);
            }
        }

        foreach (var note in filteredNotes)
        {
            Notes.Add(note);
        }
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        RefreshVisibleNotes();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}