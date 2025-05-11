using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class Favorites : Page, INotifyPropertyChanged
{
    public ObservableCollection<Notes> FavoriteNotes { get; } = new ObservableCollection<Notes>();
    private readonly DataService _dataService;

    public Favorites()
    {
        _dataService = new DataService();
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        await RefreshFavoritesAsync();
    }

    public async Task RefreshFavoritesAsync()
    {
        FavoriteNotes.Clear();
        var username = App.AuthService?.CurrentUser?.Username;
        if (!string.IsNullOrEmpty(username))
        {
            var favoriteNotes = await _dataService.LoadFavoriteNotesDetailsAsync(username);
            foreach (var note in favoriteNotes)
            {
                FavoriteNotes.Add(note);
            }
            Debug.WriteLine($"Loaded {favoriteNotes.Count} favorite notes for user {username}");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}