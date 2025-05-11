using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using practice2_OPAM_KN24_Daniel_Batko.ViewModels;
using System;
using System.Diagnostics;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class EditNotePage : Page
{
    private StorageFile? selectedImageFile;
    public EditNotesViewModel ViewModel { get; private set; }
    private readonly DataService _dataService;

    public EditNotePage()
    {
        _dataService = new DataService();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        if (e.Parameter is Notes note)
        {
            ViewModel = new EditNotesViewModel(note, _dataService);
            ImagePathText.Text = note.ImagePath != null ? System.IO.Path.GetFileName(note.ImagePath) : "The picture is not selected";

            // Configure reminder section
            ReminderCheckBox.IsChecked = note.HasReminder;
            UpdateReminderVisibility();
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        ClearErrors();

        try
        {
            if (App.AuthService?.CurrentUser == null)
            {
                ShowError(ContentErrorTextBlock, "User is not logged in. Please log in again.");
                Frame.Navigate(typeof(LoginPage));
                return;
            }

            await ViewModel.UpdateNoteAsync(
                title: ViewModel.Title,
                category: ViewModel.Category,
                content: ViewModel.Content,
                imageFile: selectedImageFile
            );

            Debug.WriteLine($"Updated note: {ViewModel.Title}");
            Frame.Navigate(typeof(AllNotes));
        }
        catch (ValidationException ex)
        {
            Debug.WriteLine($"Validation error: {ex.Message}");
            if (ex.Errors.ContainsKey(nameof(Notes.NotesTitle)))
                ShowError(TitleErrorTextBlock, string.Join("\n", ex.Errors[nameof(Notes.NotesTitle)]));
            if (ex.Errors.ContainsKey(nameof(Notes.Content)))
                ShowError(ContentErrorTextBlock, string.Join("\n", ex.Errors[nameof(Notes.Content)]));
            if (ex.Errors.ContainsKey(nameof(Notes.Category)))
                ShowError(CategoryErrorTextBlock, string.Join("\n", ex.Errors[nameof(Notes.Category)]));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unexpected error in Save_Click: {ex.Message}");
            ShowError(ContentErrorTextBlock, "An unexpected error occurred while saving the note. Please try again.");
        }
    }

    private async void PickImageButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            if (App.MainWindow != null)
            {
                var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
                InitializeWithWindow.Initialize(picker, hWnd);
                Debug.WriteLine("Successfully retrieved window handle from App.MainWindow");
            }
            else
            {
                ShowError(ImagePathText, "Main window is not available.");
                Debug.WriteLine("App.MainWindow is null in PickImageButton_Click");
                return;
            }

            selectedImageFile = await picker.PickSingleFileAsync();
            if (selectedImageFile != null)
            {
                ImagePathText.Text = selectedImageFile.Name;
                Debug.WriteLine($"Image selected: {selectedImageFile.Name}");
            }
            else
            {
                Debug.WriteLine("No image selected");
            }
        }
        catch (Exception ex)
        {
            ShowError(ImagePathText, $"Error picking image: {ex.Message}");
            Debug.WriteLine($"Error in PickImageButton_Click: {ex.Message}");
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(AllNotes));
    }

    private void ReminderCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        ReminderInputs.Visibility = Visibility.Visible;
        ViewModel.IsReminder = true;
        Debug.WriteLine("Reminder enabled");
    }

    private void ReminderCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        ReminderInputs.Visibility = Visibility.Collapsed;
        ViewModel.IsReminder = false;
        Debug.WriteLine("Reminder disabled");
    }

    private void UpdateReminderVisibility()
    {
        ReminderInputs.Visibility = ViewModel.IsReminder ? Visibility.Visible : Visibility.Collapsed;
    }

    private void ClearErrors()
    {
        TitleErrorTextBlock.Visibility = Visibility.Collapsed;
        ContentErrorTextBlock.Visibility = Visibility.Collapsed;
        CategoryErrorTextBlock.Visibility = Visibility.Collapsed;
        ImagePathText.Text = ViewModel.ImagePath != null ? System.IO.Path.GetFileName(ViewModel.ImagePath) : "The picture is not selected";
        ImagePathText.Visibility = Visibility.Visible;
    }

    private void ShowError(TextBlock errorBlock, string message)
    {
        errorBlock.Text = message;
        errorBlock.Visibility = Visibility.Visible;
    }

    private Visibility OnTitleChanged(string title)
    {
        if (string.IsNullOrEmpty(title)) return Visibility.Collapsed;

        Notes notes = new Notes();
        notes.NotesTitle = title;
        if (notes.Errors.ContainsKey(nameof(Notes.NotesTitle)))
        {
            ShowError(TitleErrorTextBlock, string.Join("\n", notes.Errors[nameof(Notes.NotesTitle)]));
            return Visibility.Visible;
        }

        return Visibility.Collapsed;
    }

    private Visibility OnCategoryChanged(string category)
    {
        if (string.IsNullOrEmpty(category)) return Visibility.Collapsed;

        Notes notes = new Notes();
        notes.Category = category;
        if (notes.Errors.ContainsKey(nameof(Notes.Category)))
        {
            ShowError(CategoryErrorTextBlock, string.Join("\n", notes.Errors[nameof(Notes.Category)]));
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }

    private Visibility OnContentChanged(string content)
    {
        if (string.IsNullOrEmpty(content)) return Visibility.Collapsed;

        Notes notes = new Notes();
        notes.Content = content;
        if (notes.Errors.ContainsKey(nameof(Notes.Content)))
        {
            ShowError(ContentErrorTextBlock, string.Join("\n", notes.Errors[nameof(Notes.Content)]));
            return Visibility.Visible;
        }
        return Visibility.Collapsed;
    }
}