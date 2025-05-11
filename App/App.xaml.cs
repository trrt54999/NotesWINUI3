using Microsoft.UI.Xaml;
using practice2_OPAM_KN24_Daniel_Batko.Services;
using System.Diagnostics;
using Windows.Storage;

namespace practice2_OPAM_KN24_Daniel_Batko;

public partial class App : Application
{
    public static AuthService AuthService { get; private set; }
    public static Window? MainWindow { get; private set; }

    public App()
    {
        InitializeComponent();
        var dataService = new DataService();
        AuthService = new AuthService(dataService);
        var localSettings = ApplicationData.Current.LocalSettings;
        var themeSetting = localSettings.Values["themeSetting"] as int?;
        if (themeSetting.HasValue)
        {
            Current.RequestedTheme = (ApplicationTheme)themeSetting.Value;
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        MainWindow = new MainWindow();
        if (MainWindow == null)
        {
            Debug.WriteLine("Failed to create MainWindow");
            return;
        }
        MainWindow.Activate();
        MainWindow.ExtendsContentIntoTitleBar = true;
    }
}