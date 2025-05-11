using Microsoft.UI;
using Microsoft.UI.Xaml;
using practice2_OPAM_KN24_Daniel_Batko.Pages;
using Windows.Graphics;
using WinRT.Interop;

namespace practice2_OPAM_KN24_Daniel_Batko;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SetupWindow();
    }

    private void SetupWindow()
    {
        var windowHandle = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
        var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);

        appWindow.Resize(new SizeInt32(800, 600));

        RootFrame.Navigate(typeof(LoginPage));
    }
}