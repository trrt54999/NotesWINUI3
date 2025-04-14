using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using practice2_OPAM_KN24_Daniel_Batko.ViewModel;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class AddNote : Page
{
    private StorageFile? selectedImageFile;
    public AddNotesViewModel ViewModel { get; } = new AddNotesViewModel();

    public AddNote()
    {
        this.InitializeComponent();

    }
    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        ClearErrors();

        try
        {
            Notes newNote = new Notes(
                 title: NotesTitle.Text,
                 category: CategoryComboBox.SelectedItem?.ToString()!,
                 content: ContentTextBox.Text
             );

            newNote.ImagePath = await SaveImageAync() ?? "ms-appx:///Assets/NotesDefaultLogo.png";
            Frame.Navigate(typeof(AllNotes), newNote);
        }
        catch (ValidationException ex)
        {
            if (ex.Errors.ContainsKey(nameof(Notes.NotesTitle)))
                ShowError(TitleErrorTextBlock, string.Join("\n", ex.Errors[nameof(Notes.NotesTitle)]));
            if (ex.Errors.ContainsKey("Content"))
                ShowError(ContentErrorTextBlock, string.Join("\n", ex.Errors["Content"]));
            if (ex.Errors.ContainsKey("Category"))
                ShowError(CategoryErrorTextBlock, string.Join("\n", ex.Errors["Category"]));
        }
    }


    private async void PickImageButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker();
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");

        var hWnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hWnd);

        selectedImageFile = await picker.PickSingleFileAsync();
        if (selectedImageFile != null)
        {
            ImagePathText.Text = selectedImageFile.Name;
        }
    }
    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(AllNotes));
    }

    private async Task<string?> SaveImageAync()
    {
        if (selectedImageFile == null) return null;

        try
        {

            var localFolder = ApplicationData.Current.LocalFolder;
            var newFile = await localFolder.CreateFileAsync(selectedImageFile.Name, CreationCollisionOption.GenerateUniqueName);
            await selectedImageFile.CopyAndReplaceAsync(newFile);
            return newFile.Path;
        }
        catch (Exception ex)
        {
            ShowError(ImagePathText, $"Error saving image: {ex.Message}");
            return null;
        }
    }

    private void ClearErrors()
    {
        TitleErrorTextBlock.Visibility = Visibility.Collapsed;
        ContentErrorTextBlock.Visibility = Visibility.Collapsed;
        CategoryErrorTextBlock.Visibility = Visibility.Collapsed;
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