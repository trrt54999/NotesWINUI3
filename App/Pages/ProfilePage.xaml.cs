using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class ProfilePage : Page
{
    private readonly DataService _dataService;

    public ProfilePage()
    {
        _dataService = new DataService();
        InitializeComponent();
        LoadUserData();
    }

    private async void ChangeAvatarButton_Click(object sender, RoutedEventArgs e)
    {
        await PickAndSaveAvatarAsync();
    }

    private async void SaveAvatarButton_Click(object sender, RoutedEventArgs e)
    {
        await SaveAvatarToCustomLocationAsync();
    }

    private async void SaveGifButton_Click(object sender, RoutedEventArgs e)
    {
        await SaveAvatarAsGifAsync();
    }

    private async void DeleteAvatarButton_Click(object sender, RoutedEventArgs e)
    {
        var confirmDialog = new ContentDialog
        {
            Title = "Delete confirmation",
            Content = "Are you sure you want to delete your avatar?",
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            DefaultButton = ContentDialogButton.Secondary,
            XamlRoot = this.XamlRoot
        };

        var result = await confirmDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await DeleteAvatarAsync();
        }
    }

    private async Task PickAndSaveAvatarAsync()
    {
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };
        picker.FileTypeFilter.Add(".jpg");
        picker.FileTypeFilter.Add(".jpeg");
        picker.FileTypeFilter.Add(".png");
        picker.FileTypeFilter.Add(".gif");

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file == null) return;

        var avatarPath = await _dataService.SaveUserAvatarAsync(file, App.AuthService.CurrentUser.Username);
        if (avatarPath == null)
        {
            await ShowError("Failed to save the avatar.");
            return;
        }

        App.AuthService.CurrentUser.AvatarPath = avatarPath;

        var users = await _dataService.LoadUsersAsync();
        var currentUser = users.FirstOrDefault(u => u.Username.Equals(App.AuthService.CurrentUser.Username, StringComparison.OrdinalIgnoreCase));
        if (currentUser != null)
        {
            currentUser.AvatarPath = avatarPath;
            await _dataService.SaveUsersAsync(users);
        }

        await UpdateProfilePictureAsync(await StorageFile.GetFileFromPathAsync(avatarPath));
    }

    private async Task SaveAvatarToCustomLocationAsync()
    {
        var currentUser = App.AuthService.CurrentUser;

        if (string.IsNullOrEmpty(currentUser.AvatarPath))
        {
            await ShowError("No avatar found to save as PNG.");
            return;
        }

        StorageFile avatarFile;
        try
        {
            avatarFile = await StorageFile.GetFileFromPathAsync(currentUser.AvatarPath);
        }
        catch
        {
            await ShowError("No avatar found to save as PNG.");
            return;
        }

        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            SuggestedFileName = $"avatar_{currentUser.Username}"
        };
        savePicker.FileTypeChoices.Add("Image", new[] { ".png" });

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(savePicker, hwnd);

        var destinationFile = await savePicker.PickSaveFileAsync();
        if (destinationFile == null) return;

        await avatarFile.CopyAndReplaceAsync(destinationFile);
    }

    private async Task SaveAvatarAsGifAsync()
    {
        var currentUser = App.AuthService.CurrentUser;

        if (string.IsNullOrEmpty(currentUser.AvatarPath))
        {
            await ShowError("No avatar found to save as GIF.");
            return;
        }

        StorageFile avatarFile;
        try
        {
            avatarFile = await StorageFile.GetFileFromPathAsync(currentUser.AvatarPath);
        }
        catch
        {
            await ShowError("No avatar found to save as GIF.");
            return;
        }

        var savePicker = new FileSavePicker
        {
            SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            SuggestedFileName = $"avatar_{currentUser.Username}"
        };
        savePicker.FileTypeChoices.Add("Image", new[] { ".gif" });

        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        InitializeWithWindow.Initialize(savePicker, hwnd);

        var destinationFile = await savePicker.PickSaveFileAsync();
        if (destinationFile == null) return;

        await avatarFile.CopyAndReplaceAsync(destinationFile);
    }

    private async Task UpdateProfilePictureAsync(StorageFile file)
    {
        using var stream = await file.OpenAsync(FileAccessMode.Read);
        var bitmapImage = new BitmapImage();
        await bitmapImage.SetSourceAsync(stream);
        ProfilePicture.ProfilePicture = bitmapImage;
    }

    private async Task DeleteAvatarAsync()
    {
        var currentUser = App.AuthService.CurrentUser;

        await _dataService.DeleteUserAvatarAsync(currentUser.Username);

        currentUser.AvatarPath = string.Empty;

        var users = await _dataService.LoadUsersAsync();
        var userToUpdate = users.FirstOrDefault(u => u.Username.Equals(currentUser.Username, StringComparison.OrdinalIgnoreCase));
        if (userToUpdate != null)
        {
            userToUpdate.AvatarPath = string.Empty;
            await _dataService.SaveUsersAsync(users);
        }

        ProfilePicture.ProfilePicture = null;
    }

    private async void LoadUserData()
    {
        var currentUser = App.AuthService.CurrentUser;

        if (currentUser == null)
        {
            Frame.Navigate(typeof(MainPage));
            return;
        }

        UsernameTextBlock.Text = currentUser.Username;
        EmailTextBlock.Text = currentUser.Email;
        FirstNameTextBlock.Text = currentUser.FirstName;
        LastNameTextBlock.Text = currentUser.LastName;
        RegistrationDateTextBlock.Text = currentUser.RegistrationDate.ToLocalTime().ToString("g");
        ProfilePicture.DisplayName = $"{currentUser.FirstName} {currentUser.LastName}";

        Debug.WriteLine($"AvatarPath: {currentUser.AvatarPath}");

        if (!string.IsNullOrEmpty(currentUser.AvatarPath))
        {
            try
            {
                var avatarFile = await StorageFile.GetFileFromPathAsync(currentUser.AvatarPath);
                await UpdateProfilePictureAsync(avatarFile);
            }
            catch
            {
                Debug.WriteLine($"Failed to load avatar from {currentUser.AvatarPath}");
            }
        }
    }

    private async Task ShowError(string message)
    {
        var errorDialog = new ContentDialog
        {
            Title = "Error",
            Content = message,
            PrimaryButtonText = "OK",
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = this.XamlRoot
        };

        await errorDialog.ShowAsync();
    }
}