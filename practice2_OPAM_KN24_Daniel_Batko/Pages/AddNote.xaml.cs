using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using practice2_OPAM_KN24_Daniel_Batko.Entities;
using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class AddNote : Page
{
    private StorageFile selectedImageFile = null;
    public AddNote()
    {
        this.InitializeComponent();

    }
    private void Save_Click(object sender, RoutedEventArgs e)
    {
        ClearErrors();

        try
        {
            Notes newNote = new Notes(
                 title: NotesTitle.Text,
                 category: CategoryComboBox.SelectedItem?.ToString()!,
                 content: ContentTextBox.Text
             );

            newNote.ImagePath = SaveImageAync().Result;
            Frame.Navigate(typeof(AllNotes), newNote);
        }
        catch (ValidationException ex)
        {
            if (ex.Errors.ContainsKey(nameof(Notes.NotesTitle)))
                ShowError(TitleError, string.Join("\n", ex.Errors[nameof(Notes.NotesTitle)]));
            if (ex.Errors.ContainsKey("Content"))
                ShowError(ContentError, string.Join("\n", ex.Errors["Content"]));
            if (ex.Errors.ContainsKey("Category"))
                ShowError(CategoryError, string.Join("\n", ex.Errors["Category"]));
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

    private async Task<string> SaveImageAync()
    {
        if (selectedImageFile == null) return null;

        var localFolder = ApplicationData.Current.LocalFolder;
        var newFile = await localFolder.CreateFileAsync(selectedImageFile.Name, CreationCollisionOption.ReplaceExisting);
        await selectedImageFile.CopyAndReplaceAsync(newFile);
        return newFile.Name;
    }

    private void ClearErrors()
    {
        TitleError.Visibility = Visibility.Collapsed;
        ContentError.Visibility = Visibility.Collapsed;
        CategoryError.Visibility = Visibility.Collapsed;
    }

    private void ShowError(TextBlock errorBlock, string message)
    {
        errorBlock.Text = message;
        errorBlock.Visibility = Visibility.Visible;
    }
}