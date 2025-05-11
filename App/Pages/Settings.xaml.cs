using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace practice2_OPAM_KN24_Daniel_Batko.Pages;

public sealed partial class Settings : Page, INotifyPropertyChanged
{
    private readonly DataService _dataService;
    private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;
    private bool _isSoundEnabled;

    public event PropertyChangedEventHandler PropertyChanged;

    public bool IsSoundEnabled
    {
        get => _isSoundEnabled;
        set
        {
            if (_isSoundEnabled != value)
            {
                _isSoundEnabled = value;
                OnPropertyChanged();
                _localSettings.Values["IsSoundEnabled"] = value;
                ElementSoundPlayer.State = value ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
            }
        }
    }

    public Settings()
    {
        InitializeComponent();
        _dataService = new DataService();
        LoadSettings();
    }

    private void LoadSettings()
    {
        var currentTheme = App.MainWindow?.Content is FrameworkElement element ? element.RequestedTheme : ElementTheme.Default;
        ThemeComboBox.SelectedItem = ThemeComboBox.Items.Cast<ComboBoxItem>()
            .FirstOrDefault(item => item.Tag.ToString() == currentTheme.ToString());

        if (_localSettings.Values.TryGetValue("IsSoundEnabled", out var isSoundEnabled))
        {
            IsSoundEnabled = (bool)isSoundEnabled;
        }
        else
        {
            IsSoundEnabled = true;
        }

        ElementSoundPlayer.State = IsSoundEnabled ? ElementSoundPlayerState.On : ElementSoundPlayerState.Off;
    }

    private void ThemeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ThemeComboBox.SelectedItem is ComboBoxItem selectedItem && App.MainWindow?.Content is FrameworkElement content)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            switch (selectedItem.Tag.ToString())
            {
                case "Light":
                    content.RequestedTheme = ElementTheme.Light;
                    localSettings.Values["themeSetting"] = 0;
                    break;
                case "Dark":
                    content.RequestedTheme = ElementTheme.Dark;
                    localSettings.Values["themeSetting"] = 1;
                    break;
                case "Default":
                    content.RequestedTheme = ElementTheme.Default;
                    localSettings.Values["themeSetting"] = 2;
                    break;
            }
        }
    }

    private async void DeleteAccountButton_Click(object sender, RoutedEventArgs e)
    {
        var confirmDialog = new ContentDialog
        {
            Title = "Confirm Account Deletion",
            Content = "Are you sure you want to delete your account? This action cannot be undone and will remove all your notes and profile data.",
            PrimaryButtonText = "Delete",
            SecondaryButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Secondary,
            XamlRoot = this.XamlRoot
        };

        var result = await confirmDialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            try
            {
                if (App.AuthService?.CurrentUser == null)
                {
                    await ShowError("No user is logged in.");
                    Frame.Navigate(typeof(LoginPage));
                    return;
                }

                await App.AuthService.DeleteAccountAsync(App.AuthService.CurrentUser.Username);
                App.AuthService.Logout();

                var successDialog = new ContentDialog
                {
                    Title = "Success",
                    Content = "Your account has been successfully deleted. The application will now close.",
                    PrimaryButtonText = "OK",
                    DefaultButton = ContentDialogButton.Primary,
                    XamlRoot = this.XamlRoot
                };
                await successDialog.ShowAsync();

                Frame.BackStack.Clear();
                Frame.Navigate(typeof(LoginPage));
                Application.Current.Exit();
            }
            catch (Exception ex)
            {
                await ShowError($"Error deleting account: {ex.Message}");
            }
        }
    }

    private async void SoundToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        var toggleSwitch = sender as ToggleSwitch;
        if (toggleSwitch != null)
        {
            Debug.WriteLine($"Sound state changed to: {toggleSwitch.IsOn}");
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

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}