using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Pages;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace practice2_OPAM_KN24_Daniel_Batko.UserControls;

public sealed partial class NotesCard : UserControl
{
    private readonly DataService _dataService;

    public Notes Notes
    {
        get => (Notes)GetValue(NotesProperty);
        set => SetValue(NotesProperty, value);
    }

    public static readonly DependencyProperty NotesProperty =
        DependencyProperty.Register(nameof(Notes), typeof(Notes), typeof(NotesCard), new PropertyMetadata(null, OnNotesChanged));

    public bool IsFavorite
    {
        get => (bool)GetValue(IsFavoriteProperty);
        set => SetValue(IsFavoriteProperty, value);
    }

    public static readonly DependencyProperty IsFavoriteProperty =
        DependencyProperty.Register(nameof(IsFavorite), typeof(bool), typeof(NotesCard), new PropertyMetadata(false));

    public NotesCard()
    {
        InitializeComponent();
        _dataService = new DataService();
        PointerPressed += NotesCard_PointerPressed;
    }

    private static async void OnNotesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NotesCard control && e.NewValue is Notes note)
        {
            control.NotesTitle.Text = note.NotesTitle;
            control.Category.Text = note.Category;
            control.Content.Text = note.Content;

            if (note.HasReminder && note.ReminderDateTime.HasValue)
            {
                control.ReminderPanel.Visibility = Visibility.Visible;
                control.ReminderTime.Text = $"{note.ReminderDateTime.Value:dd.MM.yyyy HH:mm}";
                Debug.WriteLine($"Reminder set for note {note.NotesTitle}: {note.ReminderDateTime.Value:dd.MM.yyyy HH:mm}");
            }
            else
            {
                control.ReminderPanel.Visibility = Visibility.Collapsed;
            }

            try
            {
                if (!string.IsNullOrEmpty(note.ImagePath))
                {
                    var uri = new Uri($"{note.ImagePath}?cacheBuster={Guid.NewGuid()}");
                    control.Image.Source = new BitmapImage(uri);
                    Debug.WriteLine($"Loaded image for note {note.NotesTitle}: {uri}");
                }
                else
                {
                    control.Image.Source = null;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading image for note {note.NotesTitle}: {ex.Message}");
                control.Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/note_cover.png"));
            }

            var username = App.AuthService?.CurrentUser?.Username;
            if (!string.IsNullOrEmpty(username))
            {
                var favoriteIds = await control._dataService.LoadFavoriteNotesAsync(username);
                control.IsFavorite = favoriteIds.Contains(note.Id);
            }
        }
    }

    private async void FavoriteButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var username = App.AuthService?.CurrentUser?.Username;
            if (string.IsNullOrEmpty(username))
            {
                Debug.WriteLine("No user is logged in.");
                return;
            }

            var favoriteIds = await _dataService.LoadFavoriteNotesAsync(username);
            if (IsFavorite)
            {
                favoriteIds.Remove(Notes.Id);
                IsFavorite = false;
                Debug.WriteLine($"Removed note {Notes.NotesTitle} from favorites for user {username}");
            }
            else
            {
                favoriteIds.Add(Notes.Id);
                IsFavorite = true;
                Debug.WriteLine($"Added note {Notes.NotesTitle} to favorites for user {username}");
            }
            await _dataService.SaveFavoriteNotesAsync(username, favoriteIds);

            var frame = FindParentFrame(this);
            if (frame?.Content is Favorites favoritesPage)
            {
                await favoritesPage.RefreshFavoritesAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error toggling favorite for note {Notes.NotesTitle}: {ex.Message}");
        }
    }

    private void FavoriteButton_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        FavoriteStar.Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black);
    }

    private void FavoriteButton_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        FavoriteStar.Foreground = IsFavorite
            ? new SolidColorBrush(Microsoft.UI.Colors.Gold)
            : new SolidColorBrush(Microsoft.UI.Colors.Gray);
    }

    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var confirmDialog = new ContentDialog
        {
            Title = "Confirm Deletion",
            Content = "Are you sure you want to delete this note?",
            PrimaryButtonText = "Delete",
            SecondaryButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Secondary,
            XamlRoot = XamlRoot
        };

        var result = await confirmDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await DeleteNoteAsync();
        }
    }

    private async Task DeleteNoteAsync()
    {
        try
        {
            var username = App.AuthService?.CurrentUser?.Username;
            if (string.IsNullOrEmpty(username))
            {
                Debug.WriteLine("No user is logged in.");
                return;
            }

            var notes = await _dataService.LoadNotesAsync(username);
            var noteToRemove = notes.Find(n => n.Id == Notes.Id);
            if (noteToRemove != null)
            {
                notes.Remove(noteToRemove);
                await _dataService.SaveNotesAsync(username, notes);

                if (!string.IsNullOrEmpty(noteToRemove.ImagePath) && File.Exists(noteToRemove.ImagePath))
                {
                    try
                    {
                        var imageFile = await StorageFile.GetFileFromPathAsync(noteToRemove.ImagePath);
                        await imageFile.DeleteAsync();
                        Debug.WriteLine($"Deleted image: {noteToRemove.ImagePath}");
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to delete image {noteToRemove.ImagePath}: {ex.Message}");
                    }
                }

                var favoriteIds = await _dataService.LoadFavoriteNotesAsync(username);
                if (favoriteIds.Contains(Notes.Id))
                {
                    favoriteIds.Remove(Notes.Id);
                    await _dataService.SaveFavoriteNotesAsync(username, favoriteIds);
                    Debug.WriteLine($"Removed note {Notes.NotesTitle} from favorites after deletion");
                }

                Frame frame = FindParentFrame(this);
                if (frame?.Content is AllNotes allNotesPage)
                {
                    allNotesPage.RefreshVisibleNotes();
                }
                else if (frame?.Content is Favorites favoritesPage)
                {
                    await favoritesPage.RefreshFavoritesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error deleting note: {ex.Message}");
        }
    }

    private void EditButton_Click(object sender, RoutedEventArgs e)
    {
        if (Notes != null)
        {
            Debug.WriteLine($"Navigating to EditNotePage for note: {Notes.NotesTitle}");
            Frame frame = FindParentFrame(this);
            if (frame != null)
            {
                frame.Navigate(typeof(EditNotePage), Notes);
            }
        }
    }

    private void NotesCard_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (Notes != null)
        {
            Debug.WriteLine($"Navigating to NoteDetailsPage for note: {Notes.NotesTitle}");
            Frame frame = FindParentFrame(this);
            if (frame != null)
            {
                frame.Navigate(typeof(NoteDetailsPage), Notes);
            }
        }
    }

    private Frame FindParentFrame(DependencyObject child)
    {
        DependencyObject parent = VisualTreeHelper.GetParent(child);
        while (parent != null && !(parent is Frame))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }
        return parent as Frame;
    }
}